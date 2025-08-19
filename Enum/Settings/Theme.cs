using System.ComponentModel;

namespace NBT_Studio.Enum.Settings;

public enum Theme
{
    [Description("系统")] Default,
    [Description("深色模式")] Dark,
    [Description("浅色模式")] Light
}