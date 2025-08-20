using System;
using System.ComponentModel;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace NBT_Studio.Enum.Settings;

public enum Theme
{
    [Description("系统")] Default,
    [Description("深色模式")] Dark,
    [Description("浅色模式")] Light
}

public static class ThemeExtensions
{
    public static TitleBarTheme ToTitleBarTheme(Theme theme)
    {
        return theme switch
        {
            Theme.Default => TitleBarTheme.UseDefaultAppMode,
            Theme.Dark => TitleBarTheme.Dark,
            Theme.Light => TitleBarTheme.Light,
            _ => throw new ArgumentOutOfRangeException(nameof(theme), theme, null)
        };
    }

    public static ElementTheme ToElementTheme(Theme theme)
    {
        return theme switch
        {
            Theme.Default => ElementTheme.Default,
            Theme.Dark => ElementTheme.Dark,
            Theme.Light => ElementTheme.Light,
            _ => throw new ArgumentOutOfRangeException(nameof(theme), theme, null)
        };
    }
}