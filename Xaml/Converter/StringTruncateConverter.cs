using System;
using Microsoft.UI.Xaml.Data;

namespace NBT_Studio.Xaml.Converter;

public class StringTruncateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not string str || !int.TryParse(parameter.ToString(), out var maxLength)) return value;

        return str.Length > maxLength ? str[..maxLength] + " ..." : value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}