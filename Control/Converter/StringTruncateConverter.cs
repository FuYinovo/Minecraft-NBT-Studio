using System;
using Microsoft.UI.Xaml.Data;

namespace NBT_Studio.Control.Converter;

public class StringTruncateConverter : IValueConverter
{
    private const int MaxLength = 50;

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not string str) return value;

        return str.Length > MaxLength ? str[..MaxLength] + " ..." : value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
}