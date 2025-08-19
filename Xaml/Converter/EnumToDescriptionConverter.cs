using System;
using Microsoft.UI.Xaml.Data;
using NBT_Studio.Utils;

namespace NBT_Studio.Xaml.Converter;

public class EnumToDescriptionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not System.Enum enumValue) return value;
        return ReflectionHelper.GetEnumDescription(enumValue) ?? enumValue.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}