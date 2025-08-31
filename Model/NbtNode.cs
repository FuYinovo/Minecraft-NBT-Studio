using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Enum;
using NBT_Studio.Library.NBT_Parser.Class;
using NBT_Studio.Library.NBT_Parser.Enum;
using NBT_Studio.Message;
using NBT_Studio.Service;
using NBT_Studio.Utils;
using NBT_Studio.Xaml.Control.Dialog;
using CreateNodeDialog = NBT_Studio.Xaml.Control.Dialog.CreateNodeDialog;
using RenameNodeDialog = NBT_Studio.Xaml.Control.Dialog.RenameNodeDialog;

namespace NBT_Studio.Model;

// 属性、构造方法
[SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator",
    "MVVMTK0045:Using [ObservableProperty] on fields is not AOT compatible for WinRT")]
public sealed partial class NbtNode : ObservableObject
{
    /// <summary>
    ///     初始化属性
    /// </summary>
    public NbtNode(NbtTag nbtTag, NbtNode? parent, bool isRootNode = false)
    {
        // NBT 标签
        Parent = parent;
        Tag = nbtTag;
        TagEnum = Tag.Tag;
        IsRootNode = isRootNode;
        // 名称
        DisplayName = nbtTag.Name ?? TagEnum.ToString();
        // 值
        DisplayValue = GetDisplayValue(nbtTag.Value);
        // 图标
        Icon = AssetHelper.GetNbtTagIconUri(TagEnum);
        // 节点子项
        foreach (var child in nbtTag.Children.Where(child => child.Tag != NbtTagEnum.End))
            Children.Add(new NbtNode(child, this));
        // 子项数量
        UpdateChildrenCount();
        // 是否显示子项数量
        if (NbtTagEnumExtensions.IsCollection(TagEnum)) ChildrenCountVis = Visibility.Visible;
        // 是否显示等号
        if (ChildrenCountVis == Visibility.Collapsed) EqualMarkVis = Visibility.Visible;
    }

    #region Property

    public Visibility ChildrenCountVis { get; } = Visibility.Collapsed;
    public Visibility EqualMarkVis { get; } = Visibility.Collapsed;
    public ObservableCollection<NbtNode> Children { get; } = [];
    public readonly NbtTag Tag;
    public readonly NbtNode? Parent;
    public readonly NbtTagEnum TagEnum;
    public readonly bool IsRootNode;
    public string Icon { get; }
    [ObservableProperty] private string _displayValue = string.Empty;
    [ObservableProperty] private string _displayName = string.Empty;
    [ObservableProperty] private string _displayChildrenCount = string.Empty;
    [ObservableProperty] private Visibility _visibility = Visibility.Visible;

    # endregion Property
}

// 特殊属性获取方法
public sealed partial class NbtNode
{
    /// <summary>
    ///     获取有效子项数量
    /// </summary>
    /// <remarks>节点为显示状态、未被移除则视为有效/>/></remarks>
    public int GetVisibleChildrenCount()
    {
        return Children.Count(IsValid);

        bool IsValid(NbtNode node)
        {
            return node is { Visibility: Visibility.Visible, Tag.IsRemoved: false } &&
                   node.TagEnum != NbtTagEnum.End;
        }
    }

    /// <summary>
    ///     更新「子项数量」文本
    /// </summary>
    public void UpdateChildrenCount()
    {
        if (!NbtTagEnumExtensions.IsCollection(TagEnum)) return;
        DisplayChildrenCount = $"<{GetVisibleChildrenCount()}>";
    }

    /// <summary>
    ///     获取所有子项
    /// </summary>
    public List<NbtNode> GetChildrenAll()
    {
        var got = new List<NbtNode>();
        GetChildren(this, ref got);
        return got;

        void GetChildren(NbtNode node, ref List<NbtNode> list)
        {
            list.Add(node);
            foreach (var child in node.Children.Where(c => !c.Tag.IsRemoved)) GetChildren(child, ref list);
        }
    }

    private static string GetDisplayValue(object? value)
    {
        if (value is IEnumerable enumerable and not string) return string.Join(", ", enumerable.Cast<object>());
        return value?.ToString() ?? string.Empty;
    }
}

// 更新 UI 显示方法
public sealed partial class NbtNode
{
    /// <summary>
    ///     插入子项时：更新子项列表
    /// </summary>
    /// <param name="appendedChild"></param>
    private void OnChildAppended(NbtTag appendedChild)
    {
        if (NbtTagEnumExtensions.IsCollection(TagEnum))
        {
            Children.Add(new NbtNode(appendedChild, this));
            UpdateChildrenCount();
        }
        else
        {
            // 自身为根节点时 Parent 为 null，该情况已在 IsCollection 中处理，故忽略 null 警告
            Parent!.Children.Add(new NbtNode(appendedChild, Parent));
            Parent.UpdateChildrenCount();
        }
    }

    /// <summary>
    ///     修改名称时：更新显示名称
    /// </summary>
    /// <param name="newName"></param>
    private void OnNameModified(string newName)
    {
        DisplayName = newName;
    }

    /// <summary>
    ///     修改值时：更新显示值(普通)
    /// </summary>
    private void OnValueModified(string newValue)
    {
        DisplayValue = newValue;
    }

    /// <summary>
    ///     修改值时：更新显示值(数组)
    /// </summary>
    private void OnValueModified(string[] newValue)
    {
        DisplayValue = string.Join(", ", newValue);
    }

    /// <summary>
    ///     移除时：隐藏自身、子项
    /// </summary>
    private void OnRemoved()
    {
        Visibility = Visibility.Collapsed;
        foreach (var child in Children) child.Visibility = Visibility.Collapsed;
        // 更新父节点的子项数量显示
        Parent?.UpdateChildrenCount();
    }
}

// 节点操作含界面交互的封装
public sealed partial class NbtNode
{
    /// <summary>
    ///     删除节点
    /// </summary>
    /// <remarks>涉及界面交互的封装</remarks>
    [RelayCommand]
    public async Task Delete()
    {
        if (IsRootNode)
            await NotificationService.GetInstance()
                .Send(new NotificationInfo("删除失败", "不允许删除根节点", InfoBarSeverity.Error));
        else Remove();
    }

    /// <summary>
    ///     重命名节点
    /// </summary>
    /// <remarks>涉及界面交互的封装</remarks>
    [RelayCommand]
    public async Task Rename()
    {
        var content = new RenameNodeDialog();
        var choice = await DialogHelper.ShowDialog("重命名", "确认", close: "取消", content: content);
        if (choice != ContentDialogResult.Primary) return;

        SetName(content.NewName);
    }

    /// <summary>
    ///     插入新节点
    /// </summary>
    /// <remarks>涉及界面交互的封装</remarks>
    [RelayCommand]
    public async Task AppendChild(NbtTagEnum tagEnum)
    {
        // 确认父类目标（若自身是容器，将节点插入到自身，否则插入到父项）
        var parent = NbtTagEnumExtensions.IsCollection(TagEnum)
            ? this
            : Parent!;

        // 合法性判断
        if (parent.TagEnum == NbtTagEnum.List && parent.Tag.ChildrenTag != tagEnum)
        {
            await DialogHelper.ShowDialog("添加失败", "确认",
                description: $"列表<{parent.Tag.ChildrenTag}> 不允许添加 <{tagEnum}> 节点");
            return;
        }

        var isListElement = parent.TagEnum == NbtTagEnum.List;

        // 快速创建 or 弹窗创建
        if (SettingsService.Instance.GetValue<bool>(BooleanSettings.QuickCreate))
            await QuickAppend();
        else await DetailedAppend();


        return;

        async Task DetailedAppend()
        {
            // 初始化弹窗
            var content = new CreateNodeDialog(tagEnum, isListElement);
            var title = $"添加「{ReflectionHelper.GetEnumDescription(tagEnum)}」节点";
            var dialog = DialogHelper.GetDialog(title, "确认", close: "取消", content: content);
            content.DialogOkButtonEnabledSetter = b => dialog.IsPrimaryButtonEnabled = b;
            dialog.IsPrimaryButtonEnabled = NbtTagEnumExtensions.IsCollection(tagEnum);

            // 获取输入的名称、值
            var choice = await dialog.ShowAsync();
            if (choice == ContentDialogResult.None) return;
            var name = content.GetNodeName();
            var value = content.GetNodeValue();

            AppendNewChild(tagEnum, name, value, isListElement, content.GetChildrenType(), parent);
            await SendSuccessNotification();
        }

        async Task QuickAppend()
        {
            object? value = tagEnum switch
            {
                NbtTagEnum.Byte => (byte)0,
                NbtTagEnum.Short => (short)0,
                NbtTagEnum.Int => 0,
                NbtTagEnum.Long => (long)0,
                NbtTagEnum.Float => (float)0,
                NbtTagEnum.Double => (double)0,
                NbtTagEnum.String => string.Empty,
                NbtTagEnum.ByteArray => Array.Empty<byte>(),
                NbtTagEnum.IntArray => Array.Empty<int>(),
                NbtTagEnum.LongArray => Array.Empty<long>(),
                _ => null
            };

            AppendNewChild(tagEnum, value: value, isListElement: isListElement, parentNode: parent);
            await SendSuccessNotification();
        }

        async Task SendSuccessNotification()
        {
            await NotificationService
                .GetInstance()
                .Send(new NotificationInfo(
                        "添加成功",
                        $"成功添加<{ReflectionHelper.GetEnumDescription(tagEnum)}>节点",
                        InfoBarSeverity.Success
                    )
                );
        }
    }

    /// <summary>
    ///     保存节点到单独文件
    /// </summary>
    [RelayCommand]
    public async Task Save()
    {
        // 获取保存参数
        var content = new SaveNodeDialog();
        if (await DialogHelper.ShowDialog($"单独保存「{DisplayName}」节点", "保存", close: "取消", content: content) ==
            ContentDialogResult.None) return;
        var isBigEndian = content.GameEdition == MinecraftEdition.Java;
        var compressType = content.FileCompress;

        // 选择保存位置
        var fileTypes = new Dictionary<string, List<string>> { { "NBT Files", [".nbt", ".dat"] } };
        var suggestedFileName = string.IsNullOrWhiteSpace(DisplayName)
            ? "Unnamed NBT File"
            : DisplayName;
        var file = await PickerHelper.SaveFileAsync(fileTypes, suggestedFileName);
        if (file == null) return;


        // 准备 NBT 标签
        var targetTag = (isBigEndian == Tag.IsBigEndian) switch
        {
            true => ((NbtTag)Tag.Clone()).SetEndianness(isBigEndian), // 拷贝节点
            false => Tag
        };
        if (targetTag.Tag != NbtTagEnum.Dictionary)
            // 使用一个字典标签包裹
            targetTag = new NbtTagBuilder(isBigEndian).Dictionary(string.Empty, [targetTag]);

        // 获取字节数组
        var bytes = targetTag.GetBytes();

        // 写入文件
        await using var fileStream = new FileStream(file.Path, FileMode.Create, FileAccess.Write);
        if (compressType == FileCompress.None)
        {
            await fileStream.WriteAsync(bytes);
            return;
        }

        await CompressFileHelper.CompressWriteBytes(fileStream, bytes, compressType);
        await NotificationService.GetInstance()
            .Send(new NotificationInfo("保存成功", $"成功保存「{DisplayName}」到「{file.Path}」",
                InfoBarSeverity.Success));
    }
}

// 节点操作
public sealed partial class NbtNode
{
    /// <summary>
    ///     设置节点值(普通)
    /// </summary>
    public void SetValue(string newValue)
    {
        if (NbtTagEnumExtensions.IsArray(TagEnum)) return;
        Tag.SetValue(NbtTagHelper.Parse(TagEnum, newValue));
        OnValueModified(newValue);
        SendNodeModifiedMessage(this);
    }

    /// <summary>
    ///     设置节点值(数组)
    /// </summary>
    public void SetValue(string[] newValues)
    {
        if (!NbtTagEnumExtensions.IsArray(TagEnum)) return;
        Tag.SetValue(NbtTagHelper.Parse(TagEnum, newValues));
        OnValueModified(newValues);
        SendNodeModifiedMessage(this);
    }

    /// <summary>
    ///     设置节点名称
    /// </summary>
    public void SetName(string name)
    {
        Tag.SetName(name);
        OnNameModified(name);
        SendNodeModifiedMessage(this);
    }

    /// <summary>
    ///     删除节点
    /// </summary>
    public void Remove()
    {
        Tag.RemoveSelf();
        OnRemoved();
        SendNodeModifiedMessage(this);
    }

    /// <summary>
    ///     插入节点
    /// </summary>
    /// <param name="tagEnum">插入的节点类型</param>
    /// <param name="name">节点名称></param>
    /// <param name="value">节点值</param>
    /// <param name="isListElement">父项是否为列表</param>
    /// <param name="childrenEnum">插入的节点的子项类型（默认为 Unknown）</param>
    /// <param name="parentNode">目标节点（默认为自身）</param>
    private void AppendNewChild(
        NbtTagEnum tagEnum,
        string name = "",
        object? value = null,
        bool isListElement = false,
        NbtTagEnum childrenEnum = NbtTagEnum.Unknown,
        NbtNode? parentNode = null)
    {
        if (value is null && !NbtTagEnumExtensions.IsCollection(tagEnum)) return;
        // 一、向 NBT 标签实例添加节点
        var parent = parentNode ?? this;
        var builder = new NbtTagBuilder(parent.Tag.IsBigEndian);
        var tag = tagEnum switch
        {
            // value 为 null 的情况已在开头处理
            NbtTagEnum.Byte => builder.Byte(name, (byte)value!),
            NbtTagEnum.Short => builder.Short(name, (short)value!),
            NbtTagEnum.Int => builder.Int(name, (int)value!),
            NbtTagEnum.Long => builder.Long(name, (long)value!),
            NbtTagEnum.Float => builder.Float(name, (float)value!),
            NbtTagEnum.Double => builder.Double(name, (double)value!),
            NbtTagEnum.ByteArray => builder.ByteArray(name, (byte[])value!),
            NbtTagEnum.String => builder.String(name, (string)value!),
            NbtTagEnum.List => builder.List(name, [], childrenEnum),
            NbtTagEnum.Dictionary => builder.Dictionary(name, []),
            NbtTagEnum.IntArray => builder.IntArray(name, (int[])value!),
            NbtTagEnum.LongArray => builder.LongArray(name, (long[])value!),
            _ => throw new ArgumentOutOfRangeException(nameof(tagEnum), tagEnum, null)
        };
        tag.IsListDirectElement = isListElement;
        parent.Tag.AppendChild(tag, []);
        OnChildAppended(tag);
        SendNodeModifiedMessage(this);
    }

    /// <summary>
    ///     发送「节点修改」消息
    /// </summary>
    /// <param name="node">被修改的节点</param>
    private static void SendNodeModifiedMessage(NbtNode node)
    {
        WeakReferenceMessenger.Default.Send(new NodeModifiedMessage(node));
    }
}