using System.ComponentModel;
using System.Linq;
using NBT_Studio.Attribute;

namespace NBT_Studio.Library.NBT_Parser.Enum;

public enum NbtTagEnum
{
    [Hide] Unknown = -1,
    [Hide] End = 0,
    [Order(0)] [Description("字节型")] Byte = 1,
    [Order(1)] [Description("短整型")] Short = 2,
    [Order(2)] [Description("整型")] Int = 3,
    [Order(3)] [Description("长整型")] Long = 4,
    [Order(4)] [Description("单精度浮点")] Float = 5,
    [Order(5)] [Description("双精度浮点")] Double = 6,
    [Order(7)] [Description("字节型数组")] ByteArray = 7,
    [Order(6)] [Description("字符串")] String = 8,
    [Order(10)] [Description("列表")] List = 9,
    [Order(11)] [Description("字典")] Dictionary = 10,
    [Order(8)] [Description("整型数组")] IntArray = 11,
    [Order(9)] [Description("长整型数组")] LongArray = 12
}

public static class NbtTagEnumExtensions
{
    public static readonly NbtTagEnum[] Numbers =
    [
        NbtTagEnum.Byte,
        NbtTagEnum.Short,
        NbtTagEnum.Int,
        NbtTagEnum.Long,
        NbtTagEnum.Float,
        NbtTagEnum.Double
    ];

    public static readonly NbtTagEnum[] Arrays =
    [
        NbtTagEnum.ByteArray,
        NbtTagEnum.IntArray,
        NbtTagEnum.LongArray
    ];

    public static readonly NbtTagEnum[] Collections =
    [
        NbtTagEnum.List,
        NbtTagEnum.Dictionary
    ];

    public static readonly NbtTagEnum[] Others =
    [
        NbtTagEnum.String
    ];

    public static bool IsNumber(NbtTagEnum tagEnum)
    {
        return Numbers.Contains(tagEnum);
    }

    public static bool IsCollection(NbtTagEnum tagEnum)
    {
        return Collections.Contains(tagEnum);
    }

    public static bool IsArray(NbtTagEnum tagEnum)
    {
        return Arrays.Contains(tagEnum);
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