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
    # region Private Properties

    private readonly NbtTagEnum _tagEnum;
    private readonly Brush _validBrush;
    private readonly Brush _invalidBrush;
    private readonly bool _isListElement;
    private Visibility NodeChildrenTypeVis { get; set; } = Visibility.Collapsed;
    private Visibility NodeValueVis { get; set; } = Visibility.Collapsed;
    private Visibility NodeNameVis { get; set; } = Visibility.Collapsed;
    private Visibility NodeArrayValueVis { get; set; } = Visibility.Collapsed;
    private ObservableCollection<ArrayNodeValue> NodeArrayValues { get; } = [new()];
    private ComboBoxItem SelectedChildrenTagItem { get; set; }
    private string NodeName { get; set; } = string.Empty;
    private string NodeValue { get; set; } = string.Empty;

    # endregion

    # region Public Properties

    public Action<bool>? DialogOkButtonEnabledSetter;

    public NbtTagEnum NodeChildrenType => (NbtTagEnum)SelectedChildrenTagItem.Tag;

    # endregion

    public CreateNodeDialog(NbtTagEnum tagEnum, bool isListElement)
    {
        InitializeComponent();
        _tagEnum = tagEnum;
        _isListElement = isListElement;
        _validBrush = BorderDefault.BorderBrush;
        _invalidBrush = BorderRed.BorderBrush;
        SelectedChildrenTagItem = DefaultChildrenType;
        InitVisibility();
    }

    /// <summary>
    /// 获取节点名称
    /// </summary>
    public string GetNodeName()
    {
        return NodeName;
    }

    /// <summary>
    /// 获取节点值
    /// </summary>
    public object GetNodeValue()
    {
        return NbtTagEnumExtensions.IsArray(_tagEnum)
            ? NbtTagHelper.Parse(_tagEnum, NodeArrayValues.Select(x => x.Value).ToArray())
            : NbtTagHelper.Parse(_tagEnum, NodeValue);
    }


    /// <summary>
    /// 初始化输入控件的显示状态
    /// </summary>
    private void InitVisibility()
    {
        var isCollection = NbtTagEnumExtensions.IsCollection(_tagEnum);
        var isArray = NbtTagEnumExtensions.IsArray(_tagEnum);

        if (!isCollection && !isArray) NodeValueVis = Visibility.Visible;
        if (_tagEnum == NbtTagEnum.List) NodeChildrenTypeVis = Visibility.Visible;
        if (!_isListElement) NodeNameVis = Visibility.Visible;
        if (isArray) NodeArrayValueVis = Visibility.Visible;
    }

    private void OnNodeValueChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        var isValid = NbtTagHelper.IsValueValid(_tagEnum, textBox.Text);

        UpdateBorderBrush(textBox, isValid);
        DialogOkButtonEnabledSetter?.Invoke(isValid);
    }

    private void OnNodeArrayValueChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        var arrayNodeValue = (ArrayNodeValue)textBox.DataContext;
        arrayNodeValue.IsValid =
            NbtTagHelper.IsValueValid(NbtTagEnumExtensions.GetArrayElementType(_tagEnum), textBox.Text);

        UpdateBorderBrush(textBox, arrayNodeValue.IsValid);
        DialogOkButtonEnabledSetter?.Invoke(NodeArrayValues.Count(v => v.IsValid) == NodeArrayValues.Count);
    }

    private void AddArrayValue_OnClick(object sender, RoutedEventArgs e)
    {
        NodeArrayValues.Add(new ArrayNodeValue());
        DialogOkButtonEnabledSetter?.Invoke(false);
    }

    private void UpdateBorderBrush(TextBox textBox, bool isValid)
    {
        textBox.BorderBrush = isValid ? _validBrush : _invalidBrush;
    }
}