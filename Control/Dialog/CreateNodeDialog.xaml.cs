using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using NBT_Studio.Library.NBT_Parser.Enum;
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
    private ComboBoxItem SelectedChildrenTagItem { get; set; }

    # endregion

    # region Public Properties

    public Action<bool>? DialogOkButtonEnabledSetter;

    public NbtTagEnum NodeChildrenType => (NbtTagEnum)SelectedChildrenTagItem.Tag;
    public string NodeName { get; set; } = string.Empty;
    public object NodeValue { get; set; } = string.Empty;

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


    private void InitVisibility()
    {
        if (!NbtTagEnumExtensions.IsCollection(_tagEnum)) NodeValueVis = Visibility.Visible;
        if (_tagEnum == NbtTagEnum.List) NodeChildrenTypeVis = Visibility.Visible;
        if (!_isListElement) NodeNameVis = Visibility.Visible;
    }

    private void OnNodeValueChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        var isValid = NbtValueChecker.IsValid(_tagEnum, textBox.Text);
        TNodeValue.BorderBrush = isValid ? _validBrush : _invalidBrush;
        DialogOkButtonEnabledSetter?.Invoke(isValid);
    }
}