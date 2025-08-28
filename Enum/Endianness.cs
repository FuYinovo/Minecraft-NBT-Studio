using System.ComponentModel;
using NBT_Studio.Attribute;

namespace NBT_Studio.Enum;

public enum Endianness
{
    [Description("大端序")] Big,
    [Description("小端序")] Little,
    [Hide] Unknown
}