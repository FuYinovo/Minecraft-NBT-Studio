using System.ComponentModel;

namespace NBT_Studio.Enum;

public enum Endianness
{
    [Description("大端序")] Big,
    [Description("小端序")] Little,
    Unknown
}