using System.Numerics;
using Windows.UI;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using System;

namespace NBT_Studio.Component.CustomCursor.Class;

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
        if (GetCursorColor() == color) return;
        CursorVisual.Brush = ElementCompositor.CreateColorBrush(color);
    }

    /// <summary>设置光标大小 </summary>
    public void SetCursorSize(float size)
    {
        CursorVisual.Size = new Vector2(size, size);
    }

    /// <summary>设置光标位置 </summary>
    public void SetCursorPosition(float x, float y)
    {
        CursorVisual.Offset = new Vector3(x, y, 0);
    }

    /// <summary>获取光标颜色 </summary>
    public Color? GetCursorColor()
    {
        try
        {
            return ((CompositionColorBrush)CursorVisual.Brush).Color;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>获取光标大小 </summary>
    public Vector2? GetCursorSize()
    {
        try
        {
            return CursorVisual.Size;
        }
        catch (Exception)
        {
            return null;
        }
    }
}