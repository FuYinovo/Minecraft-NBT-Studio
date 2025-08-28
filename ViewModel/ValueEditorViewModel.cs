using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Library.NBT_Parser.Enum;
using NBT_Studio.Message;
using NBT_Studio.Model;
using NBT_Studio.Utils;

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

    private void UpdateValueDisplay(TreeViewNode? treeViewNode)
    {
        if (treeViewNode == null) return;
        var node = (NbtNode)treeViewNode.Content;
        TagEnum = node.TagEnum;
        ChildrenTagEnum = node.Tag.ChildrenTag;

        var isNumber = NbtTagEnumExtensions.IsNumber(TagEnum);
        var isArray = NbtTagEnumExtensions.IsArray(TagEnum);

        Name = node.DisplayName;
        Length = node.DisplayValue.Length;
        AllowChildren = NbtTagEnumExtensions.IsCollection(TagEnum);
        AllowDelete = !node.IsRootNode;
        AllowNegative = NbtTagEnumExtensions.AllowNegative(TagEnum);
        AllowDecimal = NbtTagEnumExtensions.AllowDecimal(TagEnum);
        Type = NbtTagHelper.Instance.GetDescription(TagEnum);
        ChildrenType = NbtTagHelper.Instance.GetDescription(ChildrenTagEnum);
        MaxValue = NbtTagHelper.Instance.GetMaxValue(TagEnum);
        MinValue = NbtTagHelper.Instance.GetMinValue(TagEnum);
        ArrayValueVis = isArray ? Visibility.Visible : Visibility.Collapsed;
        GenericValueVis = isArray ? Visibility.Collapsed : Visibility.Visible;

        if (isArray && node.Tag.Value is Array values)
        {
            ArrayValues.Clear();
            foreach (var value in values)
                ArrayValues.Add(new NodeArrayElement
                    { Value = value?.ToString() ?? string.Empty, IsValid = true });
            // Task.Run(async () =>
            // {
            //     const int groupSize = 10;
            //     var added = 0;
            //     for (var i = 0; i < values.Length; i++)
            //     {
            //         if (added >= groupSize)
            //         {
            //             await Task.Delay(2000);
            //             added = 0;
            //         }
            //         ArrayValues.Add(new NodeArrayElement
            //             { Value = values.GetValue(i)?.ToString() ?? string.Empty, IsValid = true });
            //         added++;
            //     }
            // });
        }
        else
        {
            Value = node.DisplayValue;
        }


        IsChildrenTypeEnabled = TagEnum == NbtTagEnum.List;
        IsMaxValueEnabled = isNumber || isArray;
        IsMinValueEnabled = isNumber || isArray;
        IsAllowNegativeEnabled = isNumber || isArray;
        IsAllowDecimalEnabled = isNumber || isArray;
    }


    [RelayCommand]
    private void SaveValueChanges()
    {
        if (_selectedNode is null) return;
        _selectedNode.SetName(Name);
        if (NbtTagEnumExtensions.IsArray(TagEnum)) _selectedNode.SetValue(ArrayValues.Select(x => x.Value).ToArray());
        else _selectedNode.SetValue(Value);
    }

    [RelayCommand]
    private void AddArrayValue()
    {
        ArrayValues.Add(new NodeArrayElement());
    }

    [RelayCommand]
    private void RemoveArrayValue(Button button)
    {
        if (button.DataContext is not NodeArrayElement arrayValue) return;
        ArrayValues.Remove(arrayValue);
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
    [ObservableProperty] private Visibility _genericValueVis = Visibility.Collapsed;
    [ObservableProperty] private Visibility _arrayValueVis = Visibility.Collapsed;
    [ObservableProperty] private ObservableCollection<NodeArrayElement> _arrayValues = [];

    #endregion
}