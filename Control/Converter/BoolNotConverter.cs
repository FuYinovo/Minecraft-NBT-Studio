using System;
using Microsoft.UI.Xaml.Data;

namespace NBT_Studio.Control.Converter;

public class BoolNotConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not bool boolValue) return value;
        return !boolValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}