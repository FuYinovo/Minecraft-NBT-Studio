using System.ComponentModel;
using NBT_Studio.Attribute;
using WinRT;

namespace NBT_Studio.Enum;

public enum BooleanSettings
{
    [Setting(false, typeof(bool))] QuickCreate
}

public enum EnumSettings
{
    [Setting(Enum.Sort.Default, typeof(Sort))]
    Sort,

    [Setting(Enum.Theme.System, typeof(Theme))]
    Theme,
}

public enum Sort
{
    [Description("默认")] Default,
    [Description("首字母")] Alphabetical,
    [Description("类型")] Type
}

public enum Theme
{
    [Description("系统")] System,
    [Description("深色模式")] Dark,
    [Description("浅色模式")] Light
}