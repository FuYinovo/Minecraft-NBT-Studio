using System.ComponentModel;

namespace NBT_Studio.Enum;

public enum MinecraftEdition
{
    [Description("Java 版")] Java,
    [Description("基岩版")] Bedrock,
    [Description("未知")]Unknown
}