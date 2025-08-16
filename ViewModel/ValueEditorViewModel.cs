using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Library.NBT_Parser.Enum;
using NBT_Studio.Library.NBT_Parser.Utils;
using NBT_Studio.Message;
using NBT_Studio.Model;

namespace NBT_Studio.ViewModel;

[SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator",
    "MVVMTK0045:Using [ObservableProperty] on fields is not AOT compatible for WinRT")]
public sealed partial class ValueEditorViewModel : ObservableObject
{
    public ValueEditorViewModel()
    {
        RegisterMessages();
    }

    private void RegisterMessages()
    {
        // 「选中节点修改」消息
        WeakReferenceMessenger.Default.Register<SelectedNodeChangedMessage>(this,
            (_, v) =>
            {
                UpdateValueDisplay(v.Value);
                _selectedNode = (NbtNode)v.Value.Content;
            });
    }

    private void UpdateValueDisplay(TreeViewNode? value)
    {
        if (value == null) return;
        var node = (NbtNode)value.Content;
        TagEnum = node.TagEnum;
        ChildrenTagEnum = node.Tag.ChildrenTag;

        Name = node.DisplayName;
        Value = node.DisplayValue;
        Length = node.DisplayValue.Length;
        AllowChildren = NbtTagEnumExtensions.IsCollection(TagEnum);
        AllowDelete = !node.IsRootNode;
        AllowNegative = NbtTagEnumExtensions.AllowNegative(TagEnum);
        AllowDecimal = NbtTagEnumExtensions.AllowDecimal(TagEnum);
        Type = Tools.GetEnumDescription(TagEnum) ?? string.Empty;
        ChildrenType = Tools.GetEnumDescription(ChildrenTagEnum) ?? string.Empty;
        MaxValue = NbtTagEnumExtensions.GetMaxValue(TagEnum) ?? string.Empty;
        MinValue = NbtTagEnumExtensions.GetMinValue(TagEnum) ?? string.Empty;

        var isNumber = NbtTagEnumExtensions.IsNumber(TagEnum) || NbtTagEnumExtensions.IsArray(TagEnum);
        IsChildrenTypeEnabled = TagEnum == NbtTagEnum.List;
        IsMaxValueEnabled = isNumber;
        IsMinValueEnabled = isNumber;
        IsAllowNegativeEnabled = isNumber;
        IsAllowDecimalEnabled = isNumber;
    }

    [RelayCommand]
    private void SaveValueChanges()
    {
        _selectedNode?.SetValue(Value);
        _selectedNode?.SetName(Name);
    }


    #region Properties

    public NbtTagEnum TagEnum { get; private set; } = NbtTagEnum.Unknown;
    private NbtTagEnum ChildrenTagEnum { get; set; } = NbtTagEnum.Unknown;
    private NbtNode? _selectedNode;

    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _value = string.Empty;
    [ObservableProperty] private string _type = string.Empty;
    [ObservableProperty] private string _childrenType = string.Empty;
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