using Windows.UI;
using Vanara.PInvoke;
using WinRT.Interop;

namespace NBT_Studio.Utils;

public static class Win32Helper
{
    /// <summary>
    /// 获取光标处像素颜色
    /// </summary>
    public static Color GetCursorPixelColor()
    {
        User32.GetCursorPos(out var position);
        var dc = User32.GetDC(HWND.NULL);
        var pixel = Gdi32.GetPixel(dc, position.X, position.Y);
        User32.ReleaseDC(HWND.NULL, dc);
        return new Color
        {
            A = pixel.A,
            R = pixel.R,
            G = pixel.G,
            B = pixel.B
        };
    }
}