using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Control.Dialog;
using NBT_Studio.Library.NBT_Parser.Class;
using NBT_Studio.Library.NBT_Parser.Enum;
using NBT_Studio.Message;
using NBT_Studio.Service;
using NBT_Studio.Utils;

namespace NBT_Studio.Model;

// 属性、构造方法
[SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator",
    "MVVMTK0045:Using [ObservableProperty] on fields is not AOT compatible for WinRT")]
public sealed partial class NbtNode : ObservableObject
{
    private const string IconUriHead = "ms-appx:///Assets/NodeIcon/Data_node_";
    private const string IconUriExtension = ".svg";

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
        Icon = GetIconUri(TagEnum);
        // 节点子项
        foreach (var child in nbtTag.Children.Where(child => child.Tag != NbtTagEnum.End))
            Children.Add(new NbtNode(child, this));
        // 子项数量
        UpdateChildrenCount();
        // 是否显示子项数量
        if (NbtTagEnumExtensions.IsCollection(TagEnum)) ChildrenCountVis = Visibility.Visible;
        // 是否显示等号
        if (ChildrenCountVis == Visibility.Collapsed) EqualMarkVis = Visibility.Visible;

        return;


        static string GetIconUri(NbtTagEnum tagEnum)
        {
            return tagEnum switch
            {
                NbtTagEnum.ByteArray => $"{IconUriHead}byte-array{IconUriExtension}",
                NbtTagEnum.IntArray => $"{IconUriHead}int-array{IconUriExtension}",
                NbtTagEnum.LongArray => $"{IconUriHead}long-array{IconUriExtension}",
                _ => $"{IconUriHead}{tagEnum.ToString().ToLower()}{IconUriExtension}"
            };
        }
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
    [ObservableProperty] private string _displayValue;
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

        bool IsValid(NbtNode node) => node is { Visibility: Visibility.Visible, Tag.IsRemoved: false } &&
                                      node.TagEnum != NbtTagEnum.End;
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
    ///    获取所有子项
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

    private string GetDisplayValue(object? value)
    {
        if (value is IEnumerable enumerable and not string ) return string.Join(", ", enumerable.Cast<object>());
        return value?.ToString() ?? string.Empty;
    }
}

// 更新 UI 显示方法
public sealed partial class NbtNode
{
    /// <summary>
    /// 插入子项时：更新子项列表
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
    /// 修改名称时：更新显示名称
    /// </summary>
    /// <param name="newName"></param>
    private void OnNameModified(string newName)
    {
        DisplayName = newName;
    }

    /// <summary>
    /// 修改值时：更新显示值(普通)
    /// </summary>
    private void OnValueModified(string newValue)
    {
        DisplayValue = newValue;
    }

    /// <summary>
    /// 修改值时：更新显示值(数组)
    /// </summary>
    private void OnValueModified(string[] newValue)
    {
        DisplayValue = string.Join(", ", newValue);
    }

    /// <summary>
    /// 移除时：隐藏自身、子项
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
        if (IsRootNode) await DialogService.ShowDialog("删除失败", "确认", description: "不允许删除根节点");
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
        var choice = await DialogService.ShowDialog("重命名", "确认", close: "取消", content: content);
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
            await DialogService.ShowDialog("添加失败", "确认",
                description: $"列表<{parent.Tag.ChildrenTag}> 不允许添加 <{tagEnum}> 节点");
            return;
        }


        // 初始化弹窗
        var isListElement = parent.TagEnum == NbtTagEnum.List;
        var content = new CreateNodeDialog(tagEnum, isListElement);
        var dialog = DialogService.GetDialog($"添加「{tagEnum}」节点", "确认", close: "取消", content: content);
        content.DialogOkButtonEnabledSetter = b => dialog.IsPrimaryButtonEnabled = b;
        dialog.IsPrimaryButtonEnabled = NbtTagEnumExtensions.IsCollection(tagEnum);

        // 获取输入的名称、值
        var choice = await dialog.ShowAsync();
        if (choice == ContentDialogResult.None) return;
        var name = content.GetNodeName();
        var value = content.GetNodeValue();

        AppendNewChild(tagEnum, name, value, isListElement, content.NodeChildrenType, parent);
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
    /// 设置节点值(数组)
    /// </summary>
    public void SetValue(string[] newValues)
    {
        if (!NbtTagEnumExtensions.IsArray(TagEnum)) return;
        Tag.SetValue(NbtTagHelper.Parse(TagEnum, newValues));
        OnValueModified(newValues);
        SendNodeModifiedMessage(this);
    }

    /// <summary>
    /// 设置节点名称
    /// </summary>
    public void SetName(string name)
    {
        Tag.SetName(name);
        OnNameModified(name);
        SendNodeModifiedMessage(this);
    }

    /// <summary>
    /// 删除节点
    /// </summary>
    public void Remove()
    {
        Tag.RemoveSelf();
        OnRemoved();
        SendNodeModifiedMessage(this);
    }

    /// <summary>
    /// 插入节点
    /// </summary>
    /// <param name="tagEnum">插入的节点类型</param>
    /// <param name="name">节点名称></param>
    /// <param name="value">节点值</param>
    /// <param name="isListElement">父项是否为列表</param>
    /// <param name="childrenEnum">插入的节点的子项类型（默认为 Unknown）</param>
    /// <param name="parentNode">目标节点（默认为自身）</param>
    private void AppendNewChild(NbtTagEnum tagEnum, string name,
        object value, bool isListElement, NbtTagEnum childrenEnum = NbtTagEnum.Unknown, NbtNode? parentNode = null)
    {
        // 一、向 NBT 标签实例添加节点
        var parent = parentNode ?? this;
        var builder = new NbtTagBuilder(parent.Tag.IsBigEndian);
        var tag = tagEnum switch
        {
            NbtTagEnum.Byte => builder.Byte(name, (byte)value),
            NbtTagEnum.Short => builder.Short(name, (short)value),
            NbtTagEnum.Int => builder.Int(name, (int)value),
            NbtTagEnum.Long => builder.Long(name, (long)value),
            NbtTagEnum.Float => builder.Float(name, (float)value),
            NbtTagEnum.Double => builder.Double(name, (double)value),
            NbtTagEnum.ByteArray => builder.ByteArray(name, (byte[])value),
            NbtTagEnum.String => builder.String(name, (string)value),
            NbtTagEnum.List => builder.List(name, [], childrenEnum),
            NbtTagEnum.Dictionary => builder.Dictionary(name, []),
            NbtTagEnum.IntArray => builder.IntArray(name, (int[])value),
            NbtTagEnum.LongArray => builder.LongArray(name, (long[])value),
            _ => throw new ArgumentOutOfRangeException(nameof(tagEnum), tagEnum, null)
        };
        tag.IsListDirectElement = isListElement;
        parent.Tag.AppendChild(tag, []);
        OnChildAppended(tag);
        SendNodeModifiedMessage(this);
    }

    /// <summary>
    /// 发送「节点修改」消息
    /// </summary>
    /// <param name="node">被修改的节点</param>
    private static void SendNodeModifiedMessage(NbtNode node)
    {
        WeakReferenceMessenger.Default.Send(new NodeModifiedMessage(node));
    }
}