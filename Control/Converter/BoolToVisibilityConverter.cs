using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace NBT_Studio.Control.Converter;

public class BoolToVisibilityConverter:IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not bool boolean) return value;
        return boolean ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}