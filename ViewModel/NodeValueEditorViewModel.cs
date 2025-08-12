using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Library.NBT_Parser.Enum;
using NBT_Studio.Message;
using NBT_Studio.Model;

namespace NBT_Studio.ViewModel;

[SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator",
    "MVVMTK0045:Using [ObservableProperty] on fields is not AOT compatible for WinRT")]
public sealed partial class NodeValueEditorViewModel : ObservableObject
{
    public NodeValueEditorViewModel()
    {
        RegisterMessages();
    }

    private void RegisterMessages()
    {
        // 「选中节点修改」消息
        WeakReferenceMessenger.Default.Register<SelectedNodeChangedMessage>(this,
            (_, v) => UpdateValueDisplay(v.Value));
    }

    private void UpdateValueDisplay(TreeViewNode? value)
    {
        if (value == null) return;
        var node = (NbtNode)value.Content;
        TagEnum = node.TagEnum;
        ChildrenTagEnum = node.Tag.ChildrenTag;


        Name = node.Name;
        Value = node.Value;
        Length = node.Value.Length;
        AllowChildren = NbtTagEnumExtensions.IsCollection(TagEnum);
        AllowDelete = !node.IsRootNode;
        AllowNegative = NbtTagEnumExtensions.AllowNegative(TagEnum);
        AllowDecimal = NbtTagEnumExtensions.AllowDecimal(TagEnum);
        TypeIndex = GetNbtTagEnumComboBoxIndex(TagEnum);
        ChildrenTypeIndex = GetNbtTagEnumComboBoxIndex(ChildrenTagEnum);
        MaxValue = TagEnum switch
        {
            NbtTagEnum.Byte
                or NbtTagEnum.ByteArray => byte.MaxValue.ToString(),
            NbtTagEnum.Int
                or NbtTagEnum.IntArray => int.MaxValue.ToString(),
            NbtTagEnum.Long
                or NbtTagEnum.LongArray => long.MaxValue.ToString(),
            NbtTagEnum.Short => short.MaxValue.ToString(),
            NbtTagEnum.Float => float.MaxValue.ToString(),
            NbtTagEnum.Double => double.MaxValue.ToString(),
            _ => string.Empty
        };

        MinValue = TagEnum switch
        {
            NbtTagEnum.Byte
                or NbtTagEnum.ByteArray => byte.MinValue.ToString(),
            NbtTagEnum.Int
                or NbtTagEnum.IntArray => int.MinValue.ToString(),
            NbtTagEnum.Long
                or NbtTagEnum.LongArray => long.MinValue.ToString(),
            NbtTagEnum.Short => short.MinValue.ToString(),
            NbtTagEnum.Float => float.MinValue.ToString(),
            NbtTagEnum.Double => double.MinValue.ToString(),
            _ => string.Empty
        };


        var isNumber = NbtTagEnumExtensions.IsNumber(TagEnum) || NbtTagEnumExtensions.IsArray(TagEnum);
        IsChildrenTypeEnabled = TagEnum == NbtTagEnum.List;
        IsMaxValueEnabled = isNumber;
        IsMinValueEnabled = isNumber;
        IsAllowNegativeEnabled = isNumber;
        IsAllowDecimalEnabled = isNumber;

        return;

        int GetNbtTagEnumComboBoxIndex(NbtTagEnum tag)
        {
            return tag switch
            {
                NbtTagEnum.Byte => 0,
                NbtTagEnum.Short => 1,
                NbtTagEnum.Int => 2,
                NbtTagEnum.Long => 3,
                NbtTagEnum.Float => 4,
                NbtTagEnum.Double => 5,
                NbtTagEnum.ByteArray => 7,
                NbtTagEnum.String => 6,
                NbtTagEnum.List => 10,
                NbtTagEnum.Dictionary => 11,
                NbtTagEnum.IntArray => 8,
                NbtTagEnum.LongArray => 9,
                _ => -1
            };
        }
    }


    #region Properties

    public NbtTagEnum TagEnum { get; private set; } = NbtTagEnum.Unknown;
    public NbtTagEnum ChildrenTagEnum { get; private set; } = NbtTagEnum.Unknown;

    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _value = string.Empty;
    [ObservableProperty] private int _typeIndex = -1;
    [ObservableProperty] private int _childrenTypeIndex = -1;
    [ObservableProperty] private int _length;
    [ObservableProperty] private string _maxValue = string.Empty;
    [ObservableProperty] private string _minValue = string.Empty;
    [ObservableProperty] private bool _allowNegative;
    [ObservableProperty] private bool _allowDecimal;
    [ObservableProperty] private bool _allowChildren;
    [ObservableProperty] private bool _allowDelete;

    [ObservableProperty] private bool _isChildrenTypeEnabled;
    [ObservableProperty] private bool _isMaxValueEnabled;
    [ObservableProperty] private bool _isMinValueEnabled;
    [ObservableProperty] private bool _isAllowNegativeEnabled;
    [ObservableProperty] private bool _isAllowDecimalEnabled;

    #endregion
}