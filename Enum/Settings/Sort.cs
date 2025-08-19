using System.ComponentModel;

namespace NBT_Studio.Enum.Settings;

public enum Sort
{
    [Description("默认")] Default,
    [Description("首字母")] Alphabetical,
    [Description("类型")] ByType
}