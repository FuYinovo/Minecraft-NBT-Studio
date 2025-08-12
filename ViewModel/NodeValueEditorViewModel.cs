using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Library.NBT_Parser.Enum;
using NBT_Studio.Message;
using NBT_Studio.Model;

namespace NBT_Studio.ViewModel;

public sealed partial class NodeValueEditorViewModel : INotifyPropertyChanged
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

        UpdateValueSection(TagEnum);
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

    private void UpdateValueSection(NbtTagEnum tagEnum)
    {
        UnselectedScreenVisibility = Visibility.Collapsed;
        SelectedScreenVisibility = Visibility.Visible;
        if (NbtTagEnumExtensions.IsArray(tagEnum))
        {
            ArrayValueSectionVisibility = Visibility.Visible;
            GenericValueSectionVisibility = Visibility.Collapsed;
        }
        else
        {
            ArrayValueSectionVisibility = Visibility.Collapsed;
            GenericValueSectionVisibility = Visibility.Visible;
        }
    }

    #region Properties

    private Visibility _unselectedScreenVisibility = Visibility.Visible;
    private Visibility _selectedScreenVisibility = Visibility.Collapsed;
    private Visibility _genericValueSectionVisibility = Visibility.Collapsed;
    private Visibility _arrayValueSectionVisibility = Visibility.Collapsed;

    public NbtTagEnum TagEnum { get; private set; } = NbtTagEnum.Unknown;
    public NbtTagEnum ChildrenTagEnum { get; private set; } = NbtTagEnum.Unknown;

    public Visibility GenericValueSectionVisibility
    {
        get => _genericValueSectionVisibility;
        set => SetField(ref _genericValueSectionVisibility, value);
    }

    public Visibility UnselectedScreenVisibility
    {
        get => _unselectedScreenVisibility;
        private set => SetField(ref _unselectedScreenVisibility, value);
    }

    public Visibility SelectedScreenVisibility
    {
        get => _selectedScreenVisibility;
        private set => SetField(ref _selectedScreenVisibility, value);
    }

    public Visibility ArrayValueSectionVisibility
    {
        get => _arrayValueSectionVisibility;
        set => SetField(ref _arrayValueSectionVisibility, value);
    }

    #endregion

    # region NodeProperties

    private string _name = string.Empty;
    private string _value = string.Empty;
    private int _typeIndex = -1;
    private int _childrenTypeIndex = -1;
    private int _length;
    private string _maxValue = string.Empty;
    private string _minValue = string.Empty;
    private bool _allowNegative;
    private bool _allowDecimal;
    private bool _allowChildren;
    private bool _allowDelete;

    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    public string Value
    {
        get => _value;
        set => SetField(ref _value, value);
    }

    public int TypeIndex
    {
        get => _typeIndex;
        set => SetField(ref _typeIndex, value);
    }

    public int ChildrenTypeIndex
    {
        get => _childrenTypeIndex;
        set => SetField(ref _childrenTypeIndex, value);
    }

    public int Length
    {
        get => _length;
        set => SetField(ref _length, value);
    }

    public string MaxValue
    {
        get => _maxValue;
        set => SetField(ref _maxValue, value);
    }

    public string MinValue
    {
        get => _minValue;
        set => SetField(ref _minValue, value);
    }

    public bool AllowNegative
    {
        get => _allowNegative;
        set => SetField(ref _allowNegative, value);
    }

    public bool AllowDecimal
    {
        get => _allowDecimal;
        set => SetField(ref _allowDecimal, value);
    }

    public bool AllowChildren
    {
        get => _allowChildren;
        set => SetField(ref _allowChildren, value);
    }

    public bool AllowDelete
    {
        get => _allowDelete;
        set => SetField(ref _allowDelete, value);
    }

    # endregion NodeProperties

    #region NodePropertyIsEnableds

    private bool _isChildrenTypeEnabled;
    private bool _isMaxValueEnabled;
    private bool _isMinValueEnabled;
    private bool _isAllowNegativeEnabled;
    private bool _isAllowDecimalEnabled;

    public bool IsChildrenTypeEnabled
    {
        get => _isChildrenTypeEnabled;
        set => SetField(ref _isChildrenTypeEnabled, value);
    }

    public bool IsMaxValueEnabled
    {
        get => _isMaxValueEnabled;
        set => SetField(ref _isMaxValueEnabled, value);
    }

    public bool IsMinValueEnabled
    {
        get => _isMinValueEnabled;
        set => SetField(ref _isMinValueEnabled, value);
    }

    public bool IsAllowNegativeEnabled
    {
        get => _isAllowNegativeEnabled;
        set => SetField(ref _isAllowNegativeEnabled, value);
    }

    public bool IsAllowDecimalEnabled
    {
        get => _isAllowDecimalEnabled;
        set => SetField(ref _isAllowDecimalEnabled, value);
    }

    #endregion NodePropertyIsEnableds

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

    # endregion
}