using System;
using System.Linq;
using Windows.UI;

namespace NBT_Studio.Utils;

public static class ColorHelper
{
    /// <summary>
    ///     获取最近颜色
    /// </summary>
    /// <param name="color">参考色</param>
    /// <param name="allColors">所有颜色</param>
    public static Color GetClosest(Color color, Color[] allColors)
    {
        if (allColors.Length <= 1) return color;
        return allColors
            .OrderBy(x => GetColorDistance(x, color))
            .First();

        double GetColorDistance(Color color1, Color color2)
        {
            var dR = color1.R - color2.R;
            var dG = color1.G - color2.G;
            var dB = color1.B - color2.B;
            return Math.Sqrt(dR * dR + dG * dG + dB * dB);
        }
    }
}