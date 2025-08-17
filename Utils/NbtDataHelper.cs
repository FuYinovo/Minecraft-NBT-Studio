using System;
using System.Collections.Generic;
using System.Linq;
using NBT_Studio.Library.NBT_Parser.Enum;
using NBT_Studio.Library.NBT_Parser.Utils;

namespace NBT_Studio.Utils;

public class NbtDataHelper
{
    private readonly Dictionary<NbtTagEnum, string> _minValues = new();
    private readonly Dictionary<NbtTagEnum, string> _maxValues = new();
    private readonly Dictionary<NbtTagEnum, string> _descriptions = new();

    private static readonly Dictionary<NbtTagEnum, Func<string, object>> ValueParsers = new()
    {
        [NbtTagEnum.Byte] = s => byte.Parse(s),
        [NbtTagEnum.Short] = s => short.Parse(s),
        [NbtTagEnum.Int] = s => int.Parse(s),
        [NbtTagEnum.Long] = s => long.Parse(s),
        [NbtTagEnum.Float] = s => float.Parse(s),
        [NbtTagEnum.Double] = s => double.Parse(s),
        [NbtTagEnum.String] = s => s,
    };

    private static readonly Dictionary<NbtTagEnum, Func<string[], Array>> ArrayValueParsers = new()
    {
        [NbtTagEnum.ByteArray] = s => s.Select(byte.Parse).ToArray(),
        [NbtTagEnum.IntArray] = s => s.Select(int.Parse).ToArray(),
        [NbtTagEnum.LongArray] = s => s.Select(long.Parse).ToArray()
    };

    private static readonly Dictionary<NbtTagEnum, Type?> ValueTypes = new()
    {
        [NbtTagEnum.Byte] = typeof(byte),
        [NbtTagEnum.Short] = typeof(short),
        [NbtTagEnum.Int] = typeof(int),
        [NbtTagEnum.Long] = typeof(long),
        [NbtTagEnum.Float] = typeof(float),
        [NbtTagEnum.Double] = typeof(double),
        [NbtTagEnum.String] = typeof(string),
        [NbtTagEnum.ByteArray] = typeof(byte[]),
        [NbtTagEnum.IntArray] = typeof(int[]),
        [NbtTagEnum.LongArray] = typeof(long[]),
        [NbtTagEnum.Dictionary] = null,
        [NbtTagEnum.List] = null,
    };


    public static Array Parse(NbtTagEnum tagEnum, string[] tagValue)
    {
        return ArrayValueParsers[tagEnum].Invoke(tagValue);
    }

    public static object Parse(NbtTagEnum tagEnum, string tagValue)
    {
        return ValueParsers[tagEnum].Invoke(tagValue);
    }

    public string GetMinValue(NbtTagEnum tagEnum)
    {
        if (_minValues.TryGetValue(tagEnum, out var value)) return value;
        var type = ValueTypes[tagEnum];

        _minValues[tagEnum] = type switch
        {
            null => string.Empty,
            _ => NbtTagEnumExtensions.GetMinValue(tagEnum)!
        };
        return _minValues[tagEnum];
    }

    public string GetMaxValue(NbtTagEnum tagEnum)
    {
        if (_maxValues.TryGetValue(tagEnum, out var value)) return value;
        var type = ValueTypes[tagEnum];

        _maxValues[tagEnum] = type switch
        {
            null => string.Empty,
            _ => NbtTagEnumExtensions.GetMinValue(tagEnum)!
        };
        return _maxValues[tagEnum];
    }

    public string GetDescription(NbtTagEnum tagEnum)
    {
        if (_descriptions.TryGetValue(tagEnum, out var str)) return str;
        _descriptions.Add(tagEnum, Tools.GetEnumDescription(tagEnum) ?? string.Empty);
        return _descriptions[tagEnum];
    }
}