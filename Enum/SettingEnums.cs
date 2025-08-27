using System.ComponentModel;
using NBT_Studio.Attribute;

namespace NBT_Studio.Enum;

public enum BooleanSettings
{
    [Setting(false, typeof(bool))] QuickCreate
}

public enum EnumSettings
{
    [Setting(Enum.Theme.System, typeof(Theme))]
    Theme
}

public enum Theme
{
    [Description("系统")] System,
    [Description("深色模式")] Dark,
    [Description("浅色模式")] Light
}