using System;
using System.Text.RegularExpressions;
using NBT_Studio.Library.NBT_Parser.Enum;

namespace NBT_Studio.Utils;

public static class NbtValueChecker
{
    public static bool IsValid(NbtTagEnum tagEnum, string value)
    {
        if (tagEnum == NbtTagEnum.String) return true;
        return tagEnum is not (NbtTagEnum.ByteArray or NbtTagEnum.IntArray or NbtTagEnum.LongArray)
            ? IsNumberValid(tagEnum, value)
            : IsArrayValid(tagEnum, value);
    }

    private static bool IsArrayValid(NbtTagEnum tagEnum, string value)
    {
        //TODO)) 数组判断合法性
        return false;
    }

    private static bool IsNumberValid(NbtTagEnum tagEnum, string value)
    {
        if (!Regex.IsMatch(value, @"^[+-]?(\d+\.?\d*|\.\d+)$")) return false; // 是否数字
        return tagEnum switch
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
}