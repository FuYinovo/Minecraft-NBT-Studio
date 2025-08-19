using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI;
using CommunityToolkit.WinUI.Helpers;

namespace NBT_Studio.Enum;

public enum MinecraftMapColor : byte
{
    None = 0,
    Grass = 1,
    Sand = 2,
    Wool = 3,
    Fire = 4,
    Ice = 5,
    Metal = 6,
    Plant = 7,
    Snow = 8,
    Clay = 9,
    Dirt = 10,
    Stone = 11,
    Water = 12,
    Wood = 13,
    Quartz = 14,
    ColorOrange = 15,
    ColorMagenta = 16,
    ColorLightBlue = 17,
    ColorYellow = 18,
    ColorLightGreen = 19,
    ColorPink = 20,
    ColorGray = 21,
    ColorLightGray = 22,
    ColorCyan = 23,
    ColorPurple = 24,
    ColorBlue = 25,
    ColorBrown = 26,
    ColorGreen = 27,
    ColorRed = 28,
    ColorBlack = 29,
    Gold = 30,
    Diamond = 31,
    Lapis = 32,
    Emerald = 33,
    Podzol = 34,
    Nether = 35,
    TerracottaWhite = 36,
    TerracottaOrange = 37,
    TerracottaMagenta = 38,
    TerracottaLightBlue = 39,
    TerracottaYellow = 40,
    TerracottaLightGreen = 41,
    TerracottaPink = 42,
    TerracottaGray = 43,
    TerracottaLightGray = 44,
    TerracottaCyan = 45,
    TerracottaPurple = 46,
    TerracottaBlue = 47,
    TerracottaBrown = 48,
    TerracottaGreen = 49,
    TerracottaRed = 50,
    TerracottaBlack = 51,
    CrimsonNylium = 52,
    CrimsonStem = 53,
    CrimsonHyphae = 54,
    WarpedNylium = 55,
    WarpedStem = 56,
    WarpedHyphae = 57,
    WarpedWartBlock = 58,
    Deepslate = 59,
    RawIron = 60,
    GlowLichen = 61
}

public enum MinecraftColorModify : byte
{
    Low = 0,
    Normal = 1,
    High = 2,
    Lowest = 3
}

public struct MinecraftMapColorExtensions
{
    /// <summary>
    ///     获取颜色
    /// </summary>
    /// <param name="baseColor">基础色</param>
    /// <param name="modifier">修饰色</param>
    public static Color GetColor(MinecraftMapColor baseColor, MinecraftColorModify modifier)
    {
        return modifier switch
        {
            MinecraftColorModify.Low => Colors[baseColor].low,
            MinecraftColorModify.Normal => Colors[baseColor].normal,
            MinecraftColorModify.High => Colors[baseColor].high,
            MinecraftColorModify.Lowest => Colors[baseColor].lowest,
            _ => throw new ArgumentOutOfRangeException(nameof(modifier), modifier, null)
        };
    }


    /// <summary>
    /// 获取所有颜色
    /// </summary>
    public static Color[] GetAllColors()
    {
        return Colors.SelectMany(group => new[]
        {
            group.Value.lowest,
            group.Value.low,
            group.Value.normal,
            group.Value.high
        }).ToArray();
    }

    public static readonly Dictionary<MinecraftMapColor, (Color lowest, Color low, Color normal, Color high )> Colors =
        new()
        {
            {
                MinecraftMapColor.None,
                ("#00000000".ToColor(), "#00000000".ToColor(), "#00000000".ToColor(), "#00000000".ToColor())
            },
            {
                MinecraftMapColor.Grass,
                ("#435E1D".ToColor(), "#597D27".ToColor(), "#6D9930".ToColor(), "#7FB238".ToColor())
            },
            {
                MinecraftMapColor.Sand,
                ("#827B56".ToColor(), "#AEA473".ToColor(), "#D5C98C".ToColor(), "#F7E9A3".ToColor())
            },
            {
                MinecraftMapColor.Wool,
                ("#696969".ToColor(), "#8C8C8C".ToColor(), "#ABABAB".ToColor(), "#C7C7C7".ToColor())
            },
            {
                MinecraftMapColor.Fire,
                ("#870000".ToColor(), "#B40000".ToColor(), "#DC0000".ToColor(), "#FF0000".ToColor())
            },
            {
                MinecraftMapColor.Ice,
                ("#545487".ToColor(), "#7070B4".ToColor(), "#8A8ADC".ToColor(), "#A0A0FF".ToColor())
            },
            {
                MinecraftMapColor.Metal,
                ("#585858".ToColor(), "#757575".ToColor(), "#909090".ToColor(), "#A7A7A7".ToColor())
            },
            {
                MinecraftMapColor.Plant,
                ("#004100".ToColor(), "#005700".ToColor(), "#006A00".ToColor(), "#007C00".ToColor())
            },
            {
                MinecraftMapColor.Snow,
                ("#878787".ToColor(), "#B4B4B4".ToColor(), "#DCDCDC".ToColor(), "#FFFFFF".ToColor())
            },
            {
                MinecraftMapColor.Clay,
                ("#565861".ToColor(), "#737681".ToColor(), "#8D909E".ToColor(), "#A4A8B8".ToColor())
            },
            {
                MinecraftMapColor.Dirt,
                ("#4F3928".ToColor(), "#6A4C36".ToColor(), "#825E42".ToColor(), "#976D4D".ToColor())
            },
            {
                MinecraftMapColor.Stone,
                ("#3B3B3B".ToColor(), "#4F4F4F".ToColor(), "#606060".ToColor(), "#707070".ToColor())
            },
            {
                MinecraftMapColor.Water,
                ("#212187".ToColor(), "#2D2DB4".ToColor(), "#3737DC".ToColor(), "#4040FF".ToColor())
            },
            {
                MinecraftMapColor.Wood,
                ("#4B3F26".ToColor(), "#645432".ToColor(), "#7B663E".ToColor(), "#8F7748".ToColor())
            },
            {
                MinecraftMapColor.Quartz,
                ("#878581".ToColor(), "#B4B1AC".ToColor(), "#DCD9D3".ToColor(), "#FFFCF5".ToColor())
            },
            {
                MinecraftMapColor.ColorOrange,
                ("#72431B".ToColor(), "#985924".ToColor(), "#BA6D2C".ToColor(), "#D87F33".ToColor())
            },
            {
                MinecraftMapColor.ColorMagenta,
                ("#5E2872".ToColor(), "#7D3598".ToColor(), "#9941BA".ToColor(), "#B24CD8".ToColor())
            },
            {
                MinecraftMapColor.ColorLightBlue,
                ("#365172".ToColor(), "#486C98".ToColor(), "#5884BA".ToColor(), "#6699D8".ToColor())
            },
            {
                MinecraftMapColor.ColorYellow,
                ("#79791B".ToColor(), "#A1A124".ToColor(), "#C5C52C".ToColor(), "#E5E533".ToColor())
            },
            {
                MinecraftMapColor.ColorLightGreen,
                ("#436C0D".ToColor(), "#599011".ToColor(), "#6DB015".ToColor(), "#7FCC19".ToColor())
            },
            {
                MinecraftMapColor.ColorPink,
                ("#804357".ToColor(), "#AA5974".ToColor(), "#D06D8E".ToColor(), "#F27FA5".ToColor())
            },
            {
                MinecraftMapColor.ColorGray,
                ("#282828".ToColor(), "#353535".ToColor(), "#414141".ToColor(), "#4C4C4C".ToColor())
            },
            {
                MinecraftMapColor.ColorLightGray,
                ("#515151".ToColor(), "#6C6C6C".ToColor(), "#848484".ToColor(), "#999999".ToColor())
            },
            {
                MinecraftMapColor.ColorCyan,
                ("#284351".ToColor(), "#35596C".ToColor(), "#416D84".ToColor(), "#4C7F99".ToColor())
            },
            {
                MinecraftMapColor.ColorPurple,
                ("#43215E".ToColor(), "#592C7D".ToColor(), "#6D3699".ToColor(), "#7F3FB2".ToColor())
            },
            {
                MinecraftMapColor.ColorBlue,
                ("#1B285E".ToColor(), "#24357D".ToColor(), "#2C4199".ToColor(), "#334CB2".ToColor())
            },
            {
                MinecraftMapColor.ColorBrown,
                ("#36281B".ToColor(), "#483524".ToColor(), "#58412C".ToColor(), "#664C33".ToColor())
            },
            {
                MinecraftMapColor.ColorGreen,
                ("#36431B".ToColor(), "#485924".ToColor(), "#586D2C".ToColor(), "#667F33".ToColor())
            },
            {
                MinecraftMapColor.ColorRed,
                ("#511B1B".ToColor(), "#6C2424".ToColor(), "#842C2C".ToColor(), "#993333".ToColor())
            },
            {
                MinecraftMapColor.ColorBlack,
                ("#0D0D0D".ToColor(), "#111111".ToColor(), "#151515".ToColor(), "#191919".ToColor())
            },
            {
                MinecraftMapColor.Gold,
                ("#847E28".ToColor(), "#B0A836".ToColor(), "#D7CD42".ToColor(), "#FAEE4D".ToColor())
            },
            {
                MinecraftMapColor.Diamond,
                ("#307370".ToColor(), "#409A96".ToColor(), "#4FBCB7".ToColor(), "#5CDBD5".ToColor())
            },
            {
                MinecraftMapColor.Lapis,
                ("#274387".ToColor(), "#345AB4".ToColor(), "#3F6EDC".ToColor(), "#4A80FF".ToColor())
            },
            {
                MinecraftMapColor.Emerald,
                ("#00721E".ToColor(), "#009928".ToColor(), "#00BB32".ToColor(), "#00D93A".ToColor())
            },
            {
                MinecraftMapColor.Podzol,
                ("#442D19".ToColor(), "#5B3C22".ToColor(), "#6F4A2A".ToColor(), "#815631".ToColor())
            },
            {
                MinecraftMapColor.Nether,
                ("#3B0100".ToColor(), "#4F0100".ToColor(), "#600100".ToColor(), "#700200".ToColor())
            },
            {
                MinecraftMapColor.TerracottaWhite,
                ("#6E5D55".ToColor(), "#937C71".ToColor(), "#B4988A".ToColor(), "#D1B1A1".ToColor())
            },
            {
                MinecraftMapColor.TerracottaOrange,
                ("#542B13".ToColor(), "#703919".ToColor(), "#89461F".ToColor(), "#9F5224".ToColor())
            },
            {
                MinecraftMapColor.TerracottaMagenta,
                ("#4E2E39".ToColor(), "#693D4C".ToColor(), "#804B5D".ToColor(), "#95576C".ToColor())
            },
            {
                MinecraftMapColor.TerracottaLightBlue,
                ("#3B3949".ToColor(), "#4F4C61".ToColor(), "#605D77".ToColor(), "#706C8A".ToColor())
            },
            {
                MinecraftMapColor.TerracottaYellow,
                ("#624613".ToColor(), "#835D19".ToColor(), "#A0721F".ToColor(), "#BA8524".ToColor())
            },
            {
                MinecraftMapColor.TerracottaLightGreen,
                ("#363D1C".ToColor(), "#485225".ToColor(), "#58642D".ToColor(), "#677535".ToColor())
            },
            {
                MinecraftMapColor.TerracottaPink,
                ("#542829".ToColor(), "#703637".ToColor(), "#8A4243".ToColor(), "#A04D4E".ToColor())
            },
            {
                MinecraftMapColor.TerracottaGray,
                ("#1E1512".ToColor(), "#281C18".ToColor(), "#31231E".ToColor(), "#392923".ToColor())
            },
            {
                MinecraftMapColor.TerracottaLightGray,
                ("#473833".ToColor(), "#5F4B45".ToColor(), "#745C54".ToColor(), "#876B62".ToColor())
            },
            {
                MinecraftMapColor.TerracottaCyan,
                ("#2E3030".ToColor(), "#3D4040".ToColor(), "#4B4F4F".ToColor(), "#575C5C".ToColor())
            },
            {
                MinecraftMapColor.TerracottaPurple,
                ("#40262E".ToColor(), "#56333E".ToColor(), "#693E4B".ToColor(), "#7A4958".ToColor())
            },
            {
                MinecraftMapColor.TerracottaBlue,
                ("#282030".ToColor(), "#352B40".ToColor(), "#41354F".ToColor(), "#4C3E5C".ToColor())
            },
            {
                MinecraftMapColor.TerracottaBrown,
                ("#281A12".ToColor(), "#352318".ToColor(), "#412B1E".ToColor(), "#4C3223".ToColor())
            },
            {
                MinecraftMapColor.TerracottaGreen,
                ("#282B16".ToColor(), "#35391D".ToColor(), "#414624".ToColor(), "#4C522A".ToColor())
            },
            {
                MinecraftMapColor.TerracottaRed,
                ("#4B1F18".ToColor(), "#642A20".ToColor(), "#7A3327".ToColor(), "#8E3C2E".ToColor())
            },
            {
                MinecraftMapColor.TerracottaBlack,
                ("#130B08".ToColor(), "#1A0F0B".ToColor(), "#1F120D".ToColor(), "#251610".ToColor())
            },
            {
                MinecraftMapColor.CrimsonNylium,
                ("#641919".ToColor(), "#852122".ToColor(), "#A3292A".ToColor(), "#BD3031".ToColor())
            },
            {
                MinecraftMapColor.CrimsonStem,
                ("#4E2133".ToColor(), "#682C44".ToColor(), "#7F3653".ToColor(), "#943F61".ToColor())
            },
            {
                MinecraftMapColor.CrimsonHyphae,
                ("#300D0F".ToColor(), "#401114".ToColor(), "#4F1519".ToColor(), "#5C191D".ToColor())
            },
            {
                MinecraftMapColor.WarpedNylium,
                ("#0B4246".ToColor(), "#0F585E".ToColor(), "#126C73".ToColor(), "#167E86".ToColor())
            },
            {
                MinecraftMapColor.WarpedStem,
                ("#1E4B4A".ToColor(), "#286462".ToColor(), "#327A78".ToColor(), "#3A8E8C".ToColor())
            },
            {
                MinecraftMapColor.WarpedHyphae,
                ("#2D1720".ToColor(), "#3C1F2B".ToColor(), "#4A2535".ToColor(), "#562C3E".ToColor())
            },
            {
                MinecraftMapColor.WarpedWartBlock,
                ("#0A5F46".ToColor(), "#0E7F5D".ToColor(), "#119B72".ToColor(), "#14B485".ToColor())
            },
            {
                MinecraftMapColor.Deepslate,
                ("#343434".ToColor(), "#464646".ToColor(), "#565656".ToColor(), "#646464".ToColor())
            },
            {
                MinecraftMapColor.RawIron,
                ("#725C4D".ToColor(), "#987B67".ToColor(), "#BA967E".ToColor(), "#D8AF93".ToColor())
            },
            {
                MinecraftMapColor.GlowLichen,
                ("#43584F".ToColor(), "#597569".ToColor(), "#6D9081".ToColor(), "#7FA796".ToColor())
            }
        };
}