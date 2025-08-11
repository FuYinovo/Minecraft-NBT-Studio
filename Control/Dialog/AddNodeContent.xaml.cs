using System;
using System.Text.RegularExpressions;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Library.NBT_Parser.Enum;

namespace NBT_Studio.Control.Content;

public sealed partial class AddNodeContent
{
    private readonly NbtTagEnum _tagEnum;
    public Action<bool>? DialogOkButtonEnabledSetter;

    public AddNodeContent(NbtTagEnum tagEnum)
    {
        _tagEnum = tagEnum;
        InitializeComponent();
    }

    public string NodeName { get; set; } = string.Empty;
    public object NodeValue { get; set; } = string.Empty;
    public NbtTagEnum ChildrenTag { get; set; } = NbtTagEnum.Unknown;

    private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        if (!IsValid(_tagEnum, textBox.Text))
        {
            ValueTextBox.BorderBrush = RedBorder.BorderBrush;
            DialogOkButtonEnabledSetter?.Invoke(false);
        }
        else
        {
            ValueTextBox.BorderBrush = DefaultBorder.BorderBrush;
            DialogOkButtonEnabledSetter?.Invoke(true);
        }
    }


    private static bool IsValid(NbtTagEnum tagEnum, string value)
    {
        if (tagEnum == NbtTagEnum.String) return true;
        return tagEnum is not (NbtTagEnum.ByteArray or NbtTagEnum.IntArray or NbtTagEnum.LongArray)
            ? IsNumberValid(tagEnum, value)
            : IsArrayValid(tagEnum, value);
    }

    private static bool IsArrayValid(NbtTagEnum tagEnum, string value)
    {
        //TODO)) 数组判断合法性
        return false;
    }

    private static bool IsNumberValid(NbtTagEnum tagEnum, string value)
    {
        try
        {
            if (!Regex.IsMatch(value, @"^[+-]?(\d+\.?\d*|\.\d+)$")) return false; // 是否数字
            return tagEnum switch
            {
                NbtTagEnum.Byte => value is "0" or "1",
                NbtTagEnum.Short => short.TryParse(value, out _),
                NbtTagEnum.Int => int.TryParse(value, out _),
                NbtTagEnum.Long => long.TryParse(value, out _),
                NbtTagEnum.Float => float.TryParse(value, out _),
                NbtTagEnum.Double => double.TryParse(value, out _),
                _ => throw new ArgumentOutOfRangeException(nameof(tagEnum), tagEnum, null)
            };
        }
        catch (Exception)
        {
            return false;
        }
    }
}