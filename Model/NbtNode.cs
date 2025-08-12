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
using NBT_Studio.Enum;
using NBT_Studio.Library.NBT_Parser.Class;
using NBT_Studio.Library.NBT_Parser.Enum;
using NBT_Studio.Message;
using NBT_Studio.Service;
using RenameNodeContent = NBT_Studio.Control.Dialog.RenameNodeContent;

namespace NBT_Studio.Model;

// 属性、构造方法
[SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator",
    "MVVMTK0045:Using [ObservableProperty] on fields is not AOT compatible for WinRT")]
public sealed partial class NbtNode : ObservableObject
{
    /// <summary>
    ///     初始化属性
    /// </summary>
    public NbtNode(NbtTag nbtTag, bool isRootNode = false)
    {
        // NBT 标签
        Tag = nbtTag;
        TagEnum = Tag.Tag;
        IsRootNode = isRootNode;
        // 名称
        Name = nbtTag.Name ?? TagEnum.ToString();
        // 值
        var value = nbtTag.Value;
        Value = TagEnum switch
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
            Children.Add(new NbtNode(child));
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
    public NbtTagEnum TagEnum { get; }
    public string Value { get; }
    public string Icon { get; }
    public readonly bool IsRootNode;

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

// 节点操作方法
public sealed partial class NbtNode
{
    /// <summary>
    ///     删除节点
    /// </summary>
    [RelayCommand]
    private async Task Delete()
    {
        if (IsRootNode)
        {
            await DialogService.ShowDialog("删除失败", "确认", description: "不允许删除根节点");
            return;
        }

        Tag.RemoveSelf();
        WeakReferenceMessenger.Default.Send(new NodeChangedMessage((this, NodeChangeType.Remove)));
    }

    /// <summary>
    ///     重命名节点
    /// </summary>
    [RelayCommand]
    private async Task Rename()
    {
        var content = new RenameNodeContent();
        var choice = await DialogService.ShowDialog("重命名", "确认", close: "取消", content: content);
        if (choice != ContentDialogResult.Primary) return;

        Tag.SetName(content.NewName);
        Name = content.NewName;
        WeakReferenceMessenger.Default.Send(new NodeChangedMessage((this, NodeChangeType.Rename)));
    }
}