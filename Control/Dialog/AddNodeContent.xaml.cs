using System;
using System.Text.RegularExpressions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Library.NBT_Parser.Enum;

namespace NBT_Studio.Control.Dialog;

public sealed partial class AddNodeContent
{
    private readonly NbtTagEnum _tagEnum;
    public Action<bool>? DialogOkButtonEnabledSetter;

    public AddNodeContent(NbtTagEnum tagEnum)
    {
        _tagEnum = tagEnum;
        InitializeComponent();
        InitControls();
    }

    public string NodeName { get; set; } = string.Empty;
    public object NodeValue { get; set; } = string.Empty;
    public NbtTagEnum ChildrenTag => (NbtTagEnum)SelectedChildrenTagItem.Tag;
    private ComboBoxItem SelectedChildrenTagItem { get; set; } = null!;
    public NbtTagEnum SelectedChildrenTagValue { get; set; }

    private void InitControls()
    {
        SelectedChildrenTagItem = DefaultChildrenTag;
        if (_tagEnum is NbtTagEnum.Dictionary or NbtTagEnum.List) GenericValue.Visibility = Visibility.Collapsed;
        if (_tagEnum is not NbtTagEnum.List) ChildrenType.Visibility = Visibility.Collapsed;
    }

    private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        if (!Utils.NbtTagValueChecker.IsValid(_tagEnum, textBox.Text))
        {
            GenericValue.BorderBrush = BorderRed.BorderBrush;
            DialogOkButtonEnabledSetter?.Invoke(false);
        }
        else
        {
            GenericValue.BorderBrush = BorderDefault.BorderBrush;
            DialogOkButtonEnabledSetter?.Invoke(true);
        }
    }
}