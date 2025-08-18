using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using NBT_Studio.Library.NBT_Parser.Enum;
using NBT_Studio.Model;
using NBT_Studio.Utils;

namespace NBT_Studio.Control.Dialog;

public sealed partial class CreateNodeDialog
{
    public CreateNodeDialog(NbtTagEnum tagEnum, bool isListElement)
    {
        InitializeComponent();
        InitializeVisibility();
        _tagEnum = tagEnum;
        _isListElement = isListElement;
        _validBrush = BorderDefault.BorderBrush;
        _invalidBrush = BorderRed.BorderBrush;
        SelectedChildrenTagItem = DefaultChildrenType;
    }

    /// <summary>获取节点名称 </summary>
    public string GetNodeName()
    {
        return NodeName;
    }

    /// <summary> 获取节点值 </summary>
    public object GetNodeValue()
    {
        return NbtTagEnumExtensions.IsArray(_tagEnum)
            ? NbtTagHelper.Parse(_tagEnum, NodeArrayValues.Select(x => x.Value).ToArray())
            : NbtTagHelper.Parse(_tagEnum, NodeValue);
    }

    /// <summary> 获取子项类型 </summary>
    public NbtTagEnum GetChildrenType()
    {
        return (NbtTagEnum)SelectedChildrenTagItem.Tag;
    }

    /// <summary> 输入值改动 </summary>
    private void OnNodeValueChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        var isValid = NbtTagHelper.IsValueValid(_tagEnum, textBox.Text);

        UpdateBorderBrush(textBox, isValid);
        DialogOkButtonEnabledSetter?.Invoke(isValid);
    }

    /// <summary> 数组元素输入值改动 </summary>
    private void OnNodeArrayValueChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        var element = (NodeArrayElement)textBox.DataContext;
        element.IsValid =
            NbtTagHelper.IsValueValid(_tagEnum, textBox.Text);

        var isAllValid = NodeArrayValues.Count(v => v.IsValid) == NodeArrayValues.Count;
        UpdateBorderBrush(textBox, element.IsValid);
        DialogOkButtonEnabledSetter?.Invoke(isAllValid);
    }

    /// <summary> 数组添加元素 </summary>
    private void AddArrayValue_OnClick(object sender, RoutedEventArgs e)
    {
        NodeArrayValues.Add(new NodeArrayElement());
        DialogOkButtonEnabledSetter?.Invoke(false);
    }

    /// <summary> 数组删除元素 </summary>
    private void RemoveArrayValue_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: NodeArrayElement arrayValue }) return;
        NodeArrayValues.Remove(arrayValue);
    }

    /// <summary> 更新 TextBox 边框笔刷 </summary>
    private void UpdateBorderBrush(TextBox textBox, bool isValid)
    {
        textBox.BorderBrush = isValid ? _validBrush : _invalidBrush;
    }

    /// <summary> 初始化控件显示状态 </summary>
    private void InitializeVisibility()
    {
        var isCollection = NbtTagEnumExtensions.IsCollection(_tagEnum);
        var isArray = NbtTagEnumExtensions.IsArray(_tagEnum);

        if (!isCollection && !isArray) NodeValueVis = Visibility.Visible;
        if (_tagEnum == NbtTagEnum.List) NodeChildrenTypeVis = Visibility.Visible;
        if (!_isListElement) NodeNameVis = Visibility.Visible;
        if (isArray) NodeArrayValueVis = Visibility.Visible;
    }

    # region Properties

    private readonly NbtTagEnum _tagEnum;
    private readonly Brush _validBrush;
    private readonly Brush _invalidBrush;
    private readonly bool _isListElement;
    private Visibility NodeChildrenTypeVis { get; set; } = Visibility.Collapsed;
    private Visibility NodeValueVis { get; set; } = Visibility.Collapsed;
    private Visibility NodeNameVis { get; set; } = Visibility.Collapsed;
    private Visibility NodeArrayValueVis { get; set; } = Visibility.Collapsed;
    private ObservableCollection<NodeArrayElement> NodeArrayValues { get; } = [new()];
    private ComboBoxItem SelectedChildrenTagItem { get; set; }
    private string NodeName { get; set; } = string.Empty;
    private string NodeValue { get; set; } = string.Empty;

    public Action<bool>? DialogOkButtonEnabledSetter;

    # endregion
}