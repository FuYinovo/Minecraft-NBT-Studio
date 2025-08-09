using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using NBT_Studio.Library.NBT_Parser.Class;
using NBT_Studio.Library.NBT_Parser.Enum;

namespace NBT_Studio.Model;

public sealed partial class NbtNode : INotifyPropertyChanged
{
    /// <summary>
    ///     初始化属性
    /// </summary>
    public NbtNode(NbtTag nbtTag)
    {
        // NBT 标签实例
        Tag = nbtTag;
        // NBT 标签枚举
        TagEnum = Tag.Tag;
        // 名称
        Name = nbtTag.Name ?? TagEnum.ToString();
        // 值
        var unknownValue = nbtTag.Value;
        Value = TagEnum switch
        {
            NbtTagEnum.IntArray => string.Join(", ", (int[])unknownValue!),
            NbtTagEnum.ByteArray => string.Join(", ", (byte[])unknownValue!),
            NbtTagEnum.LongArray => string.Join(", ", (long[])unknownValue!),
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
                    child.TagEnum != NbtTagEnum.End && child.Visibility == Visibility.Visible);
                break;
            case NbtTagEnum.List:
                count += Children.Count(child => child.Visibility == Visibility.Visible);
                break;
        }

        return count;
    }

    /// <summary>
    ///     更新自身「子项数量」在界面上的文本
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
        foreach (var child in Children) child.GetChildren(ref got);
    }

    #region Property

    public Visibility ChildrenCountVisibility { get; } = Visibility.Collapsed;
    public Visibility EqualMarkVisibility { get; } = Visibility.Collapsed;
    public ObservableCollection<NbtNode> Children { get; } = [];
    public NbtTag Tag { get; }
    public NbtTagEnum TagEnum { get; }
    public string Name { get; }
    public string Value { get; }
    public string Icon { get; }

    #region DisplayProperty

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
        set => SetField(ref _displayChildrenCount, value);
    }

    #endregion DisplayProperty

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