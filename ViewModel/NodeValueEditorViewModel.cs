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

        Name = node.Name;
        Value = node.Value;
        Type = node.TagEnum;
        ChildrenType = node.Tag.ChildrenTag;
        Length = node.Value.Length;
        AllowChildren = NbtTagEnumExtensions.IsCollection(node.TagEnum);
        AllowDelete = !node.IsRootNode;
        AllowNegative = NbtTagEnumExtensions.AllowNegative(node.TagEnum);
        AllowDecimal = NbtTagEnumExtensions.AllowDecimal(node.TagEnum);
        MaxValue = node.TagEnum switch
        {
            NbtTagEnum.Byte => byte.MaxValue.ToString(),
            NbtTagEnum.Short => short.MaxValue.ToString(),
            NbtTagEnum.Int => int.MaxValue.ToString(),
            NbtTagEnum.Long => long.MaxValue.ToString(),
            NbtTagEnum.Float => float.MaxValue.ToString(),
            NbtTagEnum.Double => double.MaxValue.ToString(),
            _ => string.Empty
        };

        MinValue = node.TagEnum switch
        {
            NbtTagEnum.Byte => byte.MinValue.ToString(),
            NbtTagEnum.Short => short.MinValue.ToString(),
            NbtTagEnum.Int => int.MinValue.ToString(),
            NbtTagEnum.Long => long.MinValue.ToString(),
            NbtTagEnum.Float => float.MinValue.ToString(),
            NbtTagEnum.Double => double.MinValue.ToString(),
            _ => string.Empty
        };


        IsChildrenTypeEnabled = NbtTagEnumExtensions.IsCollection(Type);
        IsMaxValueEnabled = NbtTagEnumExtensions.IsNumber(Type);
        IsMinValueEnabled = NbtTagEnumExtensions.IsNumber(Type);
        IsAllowNegativeEnabled = NbtTagEnumExtensions.IsNumber(Type);
        IsAllowDecimalEnabled = NbtTagEnumExtensions.IsNumber(Type);

        UpdateValueSection(node.TagEnum);
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
    private NbtTagEnum _type = NbtTagEnum.Unknown;
    private NbtTagEnum _childrenType = NbtTagEnum.Unknown;
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

    public NbtTagEnum Type
    {
        get => _type;
        set => SetField(ref _type, value);
    }

    public NbtTagEnum ChildrenType
    {
        get => _childrenType;
        set => SetField(ref _childrenType, value);
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