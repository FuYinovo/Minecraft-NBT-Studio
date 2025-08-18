using System;
using System.Collections.Generic;
using System.Linq;
using NBT_Studio.Library.NBT_Parser.Enum;

namespace NBT_Studio.Utils;

public class NbtTagHelper
{
    private static readonly Dictionary<NbtTagEnum, Func<string, object>> ValueParsers = new()
    {
        [NbtTagEnum.Byte] = s => byte.Parse(s),
        [NbtTagEnum.Short] = s => short.Parse(s),
        [NbtTagEnum.Int] = s => int.Parse(s),
        [NbtTagEnum.Long] = s => long.Parse(s),
        [NbtTagEnum.Float] = s => float.Parse(s),
        [NbtTagEnum.Double] = s => double.Parse(s),
        [NbtTagEnum.String] = s => s
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
        [NbtTagEnum.List] = null
    };

    private readonly Dictionary<NbtTagEnum, string> _descriptions = new();
    private readonly Dictionary<NbtTagEnum, string> _maxValues = new();
    private readonly Dictionary<NbtTagEnum, string> _minValues = new();


    /// <summary>
    ///     解析数组值
    /// </summary>
    /// <param name="tagEnum">标签类型</param>
    /// <param name="tagValue">字符串的数组值</param>
    /// <returns>一个匹配对应类型的数组</returns>
    public static Array Parse(NbtTagEnum tagEnum, string[] tagValue)
    {
        return ArrayValueParsers[tagEnum].Invoke(tagValue);
    }

    /// <summary>
    ///     解析值
    /// </summary>
    /// <param name="tagEnum">标签类型</param>
    /// <param name="tagValue">字符串的值</param>
    /// <returns>匹配对应类型的值</returns>
    public static object Parse(NbtTagEnum tagEnum, string tagValue)
    {
        return ValueParsers[tagEnum].Invoke(tagValue);
    }

    /// <summary>
    ///     获取数字标签的最小值
    /// </summary>
    /// <param name="tagEnum">标签类型</param>
    /// <returns>一个字符串</returns>
    public string GetMinValue(NbtTagEnum tagEnum)
    {
        if (_minValues.TryGetValue(tagEnum, out var value)) return value;
        var type = ValueTypes[tagEnum];

        _minValues[tagEnum] = type switch
        {
            null => string.Empty,
            _ => GetMin(tagEnum)!
        };
        return _minValues[tagEnum];

        static string? GetMin(NbtTagEnum tagEnum)
        {
            return tagEnum switch
            {
                NbtTagEnum.Byte
                    or NbtTagEnum.ByteArray => byte.MinValue.ToString(),
                NbtTagEnum.Int
                    or NbtTagEnum.IntArray => int.MinValue.ToString(),
                NbtTagEnum.Long
                    or NbtTagEnum.LongArray => long.MinValue.ToString(),
                NbtTagEnum.Short => short.MinValue.ToString(),
                NbtTagEnum.Float => float.MinValue.ToString(),
                NbtTagEnum.Double => double.MinValue.ToString(),
                _ => null
            };
        }
    }

    /// <summary>
    ///     获取数字标签的最大值
    /// </summary>
    /// <param name="tagEnum">标签类型</param>
    /// <returns>一个字符串</returns>
    public string GetMaxValue(NbtTagEnum tagEnum)
    {
        if (_maxValues.TryGetValue(tagEnum, out var value)) return value;
        var type = ValueTypes[tagEnum];

        _maxValues[tagEnum] = type switch
        {
            null => string.Empty,
            _ => GetMax(tagEnum)!
        };
        return _maxValues[tagEnum];

        static string? GetMax(NbtTagEnum tagEnum)
        {
            return tagEnum switch
            {
                NbtTagEnum.Byte
                    or NbtTagEnum.ByteArray => byte.MaxValue.ToString(),
                NbtTagEnum.Int
                    or NbtTagEnum.IntArray => int.MaxValue.ToString(),
                NbtTagEnum.Long
                    or NbtTagEnum.LongArray => long.MaxValue.ToString(),
                NbtTagEnum.Short => short.MaxValue.ToString(),
                NbtTagEnum.Float => float.MaxValue.ToString(),
                NbtTagEnum.Double => double.MaxValue.ToString(),
                _ => null
            };
        }
    }

    /// <summary>
    ///     检查值的合法性
    /// </summary>
    /// <remarks>标签类型为容器则直接返回 false；字符串则直接返回 true；数组则 FallBack 到元素类型</remarks>
    /// <param name="tagEnum">标签类型</param>
    /// <param name="value">值（字符串）</param>
    public static bool IsValueValid(NbtTagEnum tagEnum, string value)
    {
        // 容器
        if (NbtTagEnumExtensions.IsCollection(tagEnum)) return false;

        // 字符串
        if (tagEnum == NbtTagEnum.String) return true;

        // 数组或其他
        var type = NbtTagEnumExtensions.IsArray(tagEnum) ? NbtTagEnumExtensions.GetArrayElementType(tagEnum) : tagEnum;
        return type switch
        {
            NbtTagEnum.Byte => byte.TryParse(value, out _),
            NbtTagEnum.Short => short.TryParse(value, out _),
            NbtTagEnum.Int => int.TryParse(value, out _),
            NbtTagEnum.Long => long.TryParse(value, out _),
            NbtTagEnum.Float => float.TryParse(value, out _),
            NbtTagEnum.Double => double.TryParse(value, out _),
            _ => throw new ArgumentOutOfRangeException(nameof(tagEnum), tagEnum, null)
        };
    }

    /// <summary>
    ///     获取标签描述
    /// </summary>
    public string GetDescription(NbtTagEnum tagEnum)
    {
        if (_descriptions.TryGetValue(tagEnum, out var str)) return str;
        _descriptions.Add(tagEnum, ReflectionHelper.GetEnumDescription(tagEnum) ?? string.Empty);
        return _descriptions[tagEnum];
    }
}