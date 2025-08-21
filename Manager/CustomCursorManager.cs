using System.Numerics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using NBT_Studio.Service;
using Compositor = Microsoft.UI.Composition.Compositor;
using SpriteVisual = Microsoft.UI.Composition.SpriteVisual;

namespace NBT_Studio.Manager;

public abstract class CustomCursorManager
{
    protected readonly SpriteVisual CursorVisual;
    protected readonly Compositor ElementCompositor;
    protected readonly UIElement TargetElement;

    protected CustomCursorManager(UIElement targetElement)
    {
        TargetElement = targetElement;
        ElementCompositor = ElementCompositionPreview.GetElementVisual(targetElement).Compositor;
        CursorVisual = CreateCursorVisual();
        HookEvents();
        SetCursorToElement();
    }

    /// <summary> 创建自定义光标</summary>
    private SpriteVisual CreateCursorVisual()
    {
        return ElementCompositor.CreateSpriteVisual();
    }

    /// <summary> 将自定义光标设置到目标元素</summary>
    private void SetCursorToElement()
    {
        ElementCompositionPreview.SetElementChildVisual(TargetElement, CursorVisual);
    }

    /// <summary>绑定光标事件到目标元素 </summary>
    private void HookEvents()
    {
        TargetElement.PointerMoved += (_, args) =>
        {
            // 跟随系统光标
            if (!CursorVisual.IsVisible) return;
            var position = args.GetCurrentPoint(TargetElement).Position;
            CursorVisual.Offset = new Vector3((float)position.X, (float)position.Y, 0);
        };
    }

    /// <summary>显示光标 </summary>
    public void ShowCursor()
    {
        CursorVisual.IsVisible = true;
    }

    /// <summary>隐藏光标 </summary>
    public void HideCursor()
    {
        CursorVisual.IsVisible = false;
    }

    /// <summary>设置光标位置 </summary>
    public void SetPosition((float x, float y) position)
    {
        CursorVisual.Offset = new Vector3(position.x, position.y, 0);
    }
}