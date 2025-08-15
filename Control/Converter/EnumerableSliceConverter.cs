using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Data;

namespace NBT_Studio.Control.Converter;

public class EnumerableSliceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not IEnumerable enumerable || parameter is not string parm) return value;
        var indexes = parm.Split(",");
        var objects = enumerable.Cast<object>().ToList();
        if (indexes.Length != 2
            || !int.TryParse(indexes.First(), out var begin)
            || !int.TryParse(indexes.Last(), out var end)
            || begin < 0
            || end > objects.Count - 1) return value;
        return objects[begin..(end + 1)];
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}