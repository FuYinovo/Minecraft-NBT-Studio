using System.ComponentModel;
using NBT_Studio.Attribute;

namespace NBT_Studio.Enum;

public enum MinecraftEdition
{
    [Description("Java 版")] Java,
    [Description("基岩版")] Bedrock,
    [Hide] Unknown
}