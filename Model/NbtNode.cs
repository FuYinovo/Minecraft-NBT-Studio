using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using NBT_Parser.Class;
using NBT_Parser.Enum;

namespace NBT_Studio.Model;

public sealed class NbtNode
{
    public Visibility ChildrenCountVisibility { get; } = Visibility.Collapsed;
    public Visibility EqualMarkVisibility { get; } = Visibility.Collapsed;
    public List<NbtNode> Children { get; } = [];
    public NbtTagEnum TagEnum { get; }
    public int ExpandButtonOpacity { get; } = 1;
    public string Name { get; }
    public string Value { get; }
    public string Icon { get; }
    public string ChildrenCount { get; }


    /// <summary>
    ///     初始化属性
    /// </summary>
    public NbtNode(NbtTag nbtTag)
    {
        TagEnum = nbtTag.Tag;
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
        ChildrenCount = $"<{Children.Count.ToString()}>";
        // 是否显示子项数量
        if (Children.Count > 0) ChildrenCountVisibility = Visibility.Visible;
        // 是否显示等号
        if (ChildrenCountVisibility == Visibility.Collapsed) EqualMarkVisibility = Visibility.Visible;
        // 折叠按钮透明度
        if (ChildrenCountVisibility == Visibility.Collapsed) ExpandButtonOpacity = 0;

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
}