using System.Numerics;
using Windows.UI;
using Microsoft.UI;
using Microsoft.UI.Xaml;

namespace NBT_Studio.Service;

public class SquareCursorManager : CustomCursorManager
{
    public SquareCursorManager(UIElement targetElement, Color? cursorColor = null, int size = 16) : base(targetElement)
    {
        SetCursorColor(cursorColor ?? Colors.White);
        SetCursorSize(size);
    }

    public bool IsVisible => CursorVisual.IsVisible;

    /// <summary>设置光标颜色 </summary>
    public void SetCursorColor(Color color)
    {
        CursorVisual.Brush = ElementCompositor.CreateColorBrush(color);
    }

    /// <summary>设置光标大小 </summary>
    public void SetCursorSize(float size)
    {
        CursorVisual.Size = new Vector2(size, size);
    }
}