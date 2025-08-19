using System;
using Microsoft.UI.Xaml.Data;

namespace NBT_Studio.Xaml.Converter;

/// <summary>
///     将一个长度大于 50 的字符串截断
/// </summary>
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
        throw new NotImplementedException();
    }
}