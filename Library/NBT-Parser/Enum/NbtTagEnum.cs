using System;
using System.ComponentModel;

namespace NBT_Studio.Library.NBT_Parser.Enum;

public enum NbtTagEnum
{
    Unknown = -1,
    End = 0,
    [Description("字节")] Byte = 1,
    [Description("短整型")] Short = 2,
    [Description("整型")] Int = 3,
    [Description("长整型")] Long = 4,
    [Description("单精度浮点")] Float = 5,
    [Description("双精度浮点")] Double = 6,
    [Description("字节数组")] ByteArray = 7,
    [Description("字符串")] String = 8,
    [Description("列表")] List = 9,
    [Description("字典")] Dictionary = 10,
    [Description("整型数组")] IntArray = 11,
    [Description("长整型数组")] LongArray = 12
}

public static class NbtTagEnumExtensions
{
    public static string? GetMaxValue(NbtTagEnum tagEnum)
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

    public static string? GetMinValue(NbtTagEnum tagEnum)
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

    public static bool IsNumber(NbtTagEnum tagEnum)
    {
        return tagEnum switch
        {
            NbtTagEnum.Byte or
                NbtTagEnum.Short or
                NbtTagEnum.Int or
                NbtTagEnum.Long or
                NbtTagEnum.Float or
                NbtTagEnum.Double => true,
            _ => false
        };
    }

    public static bool IsCollection(NbtTagEnum tagEnum)
    {
        return tagEnum switch
        {
            NbtTagEnum.Dictionary or
                NbtTagEnum.List => true,
            _ => false
        };
    }

    public static bool IsArray(NbtTagEnum tagEnum)
    {
        return tagEnum switch
        {
            NbtTagEnum.ByteArray or
                NbtTagEnum.IntArray or
                NbtTagEnum.LongArray => true,
            _ => false
        };
    }


    public static bool AllowDecimal(NbtTagEnum tagEnum)
    {
        if (!IsNumber(tagEnum)) return false;
        return tagEnum switch
        {
            NbtTagEnum.Float or
                NbtTagEnum.Double => true,
            _ => false
        };
    }

    public static bool AllowNegative(NbtTagEnum tagEnum)
    {
        if (!IsNumber(tagEnum)) return false;
        return tagEnum switch
        {
            NbtTagEnum.Byte => false,
            _ => true
        };
    }

    public static NbtTagEnum GetArrayElementType(NbtTagEnum tagEnum)
    {
        return tagEnum switch
        {
            NbtTagEnum.ByteArray => NbtTagEnum.Byte,
            NbtTagEnum.IntArray => NbtTagEnum.Int,
            NbtTagEnum.LongArray => NbtTagEnum.Long,
            _ => NbtTagEnum.Unknown
        };
    }
}