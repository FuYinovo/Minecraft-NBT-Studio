using System;
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
using NBT_Studio.Enum;
using NBT_Studio.Library.NBT_Parser.Class;
using NBT_Studio.Library.NBT_Parser.Enum;
using NBT_Studio.Message;
using NBT_Studio.Service;

namespace NBT_Studio.Model;

// 属性、构造方法
[SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator",
    "MVVMTK0045:Using [ObservableProperty] on fields is not AOT compatible for WinRT")]
public sealed partial class NbtNode : ObservableObject
{
    /// <summary>
    ///     初始化属性
    /// </summary>
    public NbtNode(NbtTag nbtTag, bool isRootNode = false, NbtNode? parent = null)
    {
        // NBT 标签
        Parent = parent;
        Tag = nbtTag;
        TagEnum = Tag.Tag;
        IsRootNode = isRootNode;
        // 名称
        Name = nbtTag.Name ?? TagEnum.ToString();
        // 值
        var value = nbtTag.Value;
        DisplayValue = TagEnum switch
        {
            NbtTagEnum.IntArray => string.Join(", ", (int[])value!),
            NbtTagEnum.ByteArray => string.Join(", ", (byte[])value!),
            NbtTagEnum.LongArray => string.Join(", ", (long[])value!),
            _ => nbtTag.Value?.ToString() ?? ""
        };
        // 图标
        Icon = GetIconUri(TagEnum);
        // 节点子项
        foreach (var child in nbtTag.Children.Where(child => child.Tag != NbtTagEnum.End))
            Children.Add(new NbtNode(child, false, this));
        // 子项数量
        DisplayChildrenCount = $"<{Children.Count.ToString()}>";
        // 是否显示子项数量
        if (NbtTagEnumExtensions.IsCollection(TagEnum)) ChildrenCountVisibility = Visibility.Visible;
        // 是否显示等号
        if (ChildrenCountVisibility == Visibility.Collapsed) EqualMarkVisibility = Visibility.Visible;

        return;


        static string GetIconUri(NbtTagEnum tagEnum)
        {
            const string path = "ms-appx:///Assets/NodeIcon/";
            const string head = "Data_node_";
            const string extension = ".svg";
            return tagEnum switch
            {
                NbtTagEnum.ByteArray => $"{path}{head}byte-array{extension}",
                NbtTagEnum.IntArray => $"{path}{head}int-array{extension}",
                NbtTagEnum.LongArray => $"{path}{head}long-array{extension}",
                _ => $"{path}{head}{tagEnum.ToString().ToLower()}{extension}"
            };
        }
    }

    #region Property

    public Visibility ChildrenCountVisibility { get; } = Visibility.Collapsed;
    public Visibility EqualMarkVisibility { get; } = Visibility.Collapsed;
    public ObservableCollection<NbtNode> Children { get; } = [];
    public NbtTag Tag { get; }
    public NbtNode? Parent { get; }
    public NbtTagEnum TagEnum { get; }
    public string Icon { get; }
    public readonly bool IsRootNode;
    [ObservableProperty] private string _displayValue;
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _displayChildrenCount = string.Empty;
    [ObservableProperty] private Visibility _visibility = Visibility.Visible;

    # endregion Property
}

// 特殊属性获取方法
public sealed partial class NbtNode
{
    /// <summary>
    ///     获取自身可见子项的数量
    /// </summary>
    public int GetVisibleChildrenCount()
    {
        var count = 0;
        switch (TagEnum)
        {
            case NbtTagEnum.Dictionary:
                count += Children.Count(child =>
                    child.TagEnum != NbtTagEnum.End && child is
                        { Visibility: Visibility.Visible, Tag.IsRemoved: false });
                break;
            case NbtTagEnum.List:
                count += Children.Count(child => child is { Visibility: Visibility.Visible, Tag.IsRemoved: false });
                break;
        }

        return count;
    }

    /// <summary>
    ///     更新自身「子项数量」文本
    /// </summary>
    public void UpdateChildrenCount()
    {
        if (!NbtTagEnumExtensions.IsCollection(TagEnum)) return;
        DisplayChildrenCount = $"<{GetVisibleChildrenCount()}>";
    }

    /// <summary>
    ///     以自身为根节点，获取所有子项
    /// </summary>
    public List<NbtNode> GetChildrenAll()
    {
        var got = new List<NbtNode>();
        GetChildren(ref got);
        return got;
    }

    /// <summary>
    ///     GetChildrenAll 的实现方法
    /// </summary>
    /// <remarks>递归方法</remarks>
    /// <param name="got">已取得的子项列表</param>
    private void GetChildren(ref List<NbtNode> got)
    {
        got.Add(this);
        foreach (var child in Children.Where(node => !node.Tag.IsRemoved)) child.GetChildren(ref got);
    }
}

// 数据测的节点操作
public sealed partial class NbtNode
{
    /// <summary>
    ///     删除节点
    /// </summary>
    /// <remarks>数据测</remarks>
    [RelayCommand]
    private async Task Delete()
    {
        if (IsRootNode)
        {
            await DialogService.ShowDialog("删除失败", "确认", description: "不允许删除根节点");
            return;
        }

        Tag.RemoveSelf();
        WeakReferenceMessenger.Default.Send(new ModifyNodeMessage((this, NodeModify.Remove)));
    }

    /// <summary>
    ///     重命名节点
    /// </summary>
    /// <remarks>数据测</remarks>
    [RelayCommand]
    private async Task Rename()
    {
        var content = new RenameNodeDialog();
        var choice = await DialogService.ShowDialog("重命名", "确认", close: "取消", content: content);
        if (choice != ContentDialogResult.Primary) return;

        Tag.SetName(content.NewName);
        Name = content.NewName;
        WeakReferenceMessenger.Default.Send(new ModifyNodeMessage((this, NodeModify.Rename)));
    }

    /// <summary>
    ///     在自身或父类插入新节点
    /// </summary>
    /// <remarks>数据测与界面测实现均在 TreeViewPageViewModel </remarks>
    [RelayCommand]
    private void CreateChild(NbtTagEnum tagEnum)
    {
        WeakReferenceMessenger.Default.Send(new CreateNodeMessage(this, tagEnum));
    }

    /// <summary>
    ///     设置节点值
    /// </summary>
    /// <remarks>数据测</remarks>
    public void SetValue(string newValue)
    {
        Tag.SetValue(TagEnum switch
        {
            //TODO)) 数组保存
            NbtTagEnum.Byte => byte.Parse(newValue),
            NbtTagEnum.Short => short.Parse(newValue),
            NbtTagEnum.Int => int.Parse(newValue),
            NbtTagEnum.Long => long.Parse(newValue),
            NbtTagEnum.Float => float.Parse(newValue),
            NbtTagEnum.Double => double.Parse(newValue),
            NbtTagEnum.ByteArray => Array.Empty<byte>(),
            NbtTagEnum.String => newValue,
            NbtTagEnum.IntArray => Array.Empty<int>(),
            NbtTagEnum.LongArray => Array.Empty<long>(),
            _ => throw new ArgumentOutOfRangeException()
        });

        DisplayValue = newValue;
        WeakReferenceMessenger.Default.Send(new ModifyNodeMessage((this, NodeModify.SetValue)));
    }
}