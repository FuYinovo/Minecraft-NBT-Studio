using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Control.Content;
using NBT_Studio.Enum;
using NBT_Studio.Library.NBT_Parser.Class;
using NBT_Studio.Library.NBT_Parser.Enum;
using NBT_Studio.Service;

namespace NBT_Studio.Model;

// 属性、构造方法
public sealed partial class NbtNode : INotifyPropertyChanged
{
    /// <summary>
    ///     初始化属性
    /// </summary>
    public NbtNode(NbtTag nbtTag, bool isRootNode = false,
        Action<NbtNode, NodeChangeType>? applyNodeChangeWithUiRequest = null)
    {
        _applyNodeChangeWithUiRequest = applyNodeChangeWithUiRequest;

        // Command
        DeleteCommand = new AsyncRelayCommand(Delete);
        RenameCommand = new AsyncRelayCommand(Rename);
        // NBT 标签
        Tag = nbtTag;
        TagEnum = Tag.Tag;
        _isRootNode = isRootNode;
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
            Children.Add(new NbtNode(child, false, _applyNodeChangeWithUiRequest));
        // 子项数量
        DisplayChildrenCount = $"<{Children.Count.ToString()}>";
        // 是否显示子项数量
        if (TagEnum is NbtTagEnum.Dictionary or NbtTagEnum.List) ChildrenCountVisibility = Visibility.Visible;
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
    public IAsyncRelayCommand DeleteCommand { get; }
    public IAsyncRelayCommand RenameCommand { get; }
    public NbtTag Tag { get; }
    public NbtTagEnum TagEnum { get; }
    public string Value { get; }
    public string Icon { get; }
    private readonly Action<NbtNode, NodeChangeType>? _applyNodeChangeWithUiRequest;
    private readonly bool _isRootNode;
    private string _name = string.Empty;

    private string _displayChildrenCount = string.Empty;
    private Visibility _visibility = Visibility.Visible;

    public Visibility Visibility
    {
        get => _visibility;
        set => SetField(ref _visibility, value);
    }

    public string DisplayChildrenCount
    {
        get => _displayChildrenCount;
        private set => SetField(ref _displayChildrenCount, value);
    }

    public string Name
    {
        get => _name;
        private set => SetField(ref _name, value);
    }

    # endregion Property

    # region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(propertyName);
    }

    #endregion
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
        if (TagEnum is not (NbtTagEnum.Dictionary or NbtTagEnum.List)) return;
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
    private async Task Delete()
    {
        if (_isRootNode)
        {
            await DialogService.ShowDialog("删除失败", "确认", description: "不允许删除根节点");
            return;
        }

        Tag.RemoveSelf();
        _applyNodeChangeWithUiRequest?.Invoke(this, NodeChangeType.Remove);
    }

    /// <summary>
    ///     重命名节点
    /// </summary>
    private async Task Rename()
    {
        var content = new RenameNodeContent();
        var choice = await DialogService.ShowDialog("重命名", "确认", close: "取消", content: content);
        if (choice != ContentDialogResult.Primary) return;

        Tag.SetName(content.NewName);
        Name = content.NewName;
        _applyNodeChangeWithUiRequest?.Invoke(this, NodeChangeType.Rename);
    }
}