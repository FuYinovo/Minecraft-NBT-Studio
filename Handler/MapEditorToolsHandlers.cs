using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Microsoft.UI;
using Microsoft.UI.Xaml.Input;
using NBT_Studio.Attribute;
using NBT_Studio.Enum;
using NBT_Studio.Interface;
using NBT_Studio.Utils;
using NBT_Studio.View;

namespace NBT_Studio.Handler;

public record MapEditorToolsHandlers
{
    /// <summary> 选择 </summary>
    [MapEditorToolHandler(Tool = MapEditorTool.Select)]
    public class SelectHandler(MapEditor editor) : IMapEditorPointerEventsHandler
    {
        public MapEditor Editor { get; set; } = editor;
    }

    /// <summary> 橡皮 </summary>
    /// <remarks> 继承笔刷所有方法，仅 SetColor 替换为透明色 </remarks>
    [MapEditorToolHandler(Tool = MapEditorTool.Eraser)]
    public class EraserHandler(MapEditor editor) : BrushHandler(editor)
    {
        protected override void SetColor(int x, int y, Color color)
        {
            Editor.ViewModel.SetColor(x, y, Colors.Transparent);
        }
    }

    /// <summary> 吸色器 </summary>
    [MapEditorToolHandler(Tool = MapEditorTool.ColorPicker)]
    public class ColorPickerHandler(MapEditor editor) : IMapEditorPointerEventsHandler
    {
        public MapEditor Editor { get; set; } = editor;

        public void HandleEntered(object sender, PointerRoutedEventArgs args)
        {
            Editor.SetCursor(CoreCursorType.Cross);
        }

        public void HandleExited(object sender, PointerRoutedEventArgs args)
        {
            Editor.SetCursor(CoreCursorType.Arrow);
        }

        public void HandlePressed(object sender, PointerRoutedEventArgs args)
        {
            Editor.ViewModel.BrushColor = Win32Helper.GetCursorPixelColor();
        }
    }

    /// <summary> 缩放 </summary>
    [MapEditorToolHandler(Tool = MapEditorTool.Zoom)]
    public class ZoomHandler(MapEditor editor) : IMapEditorPointerEventsHandler
    {
        public MapEditor Editor { get; set; } = editor;
    }

    /// <summary> 移动 </summary>
    [MapEditorToolHandler(Tool = MapEditorTool.Move)]
    public class MoveHandler(MapEditor editor) : IMapEditorPointerEventsHandler
    {
        public MapEditor Editor { get; set; } = editor;
        private Point _originPoint;
        private const float MovingSpeedFactor = (float)0.05;

        public void HandleMoved(object sender, PointerRoutedEventArgs args)
        {
            if (!Editor.IsMousePressing) return;
            var position = GetMousePosition(args);
            var maxOffset = MapEditor.InitMapSize * Editor.MapScale;

            (float x, float y) extraOffset = (
                MovingSpeedFactor * (position._x - _originPoint._x),
                MovingSpeedFactor * (position._y - _originPoint._y));

            if (!(Editor.MapOffset.x + extraOffset.x > maxOffset))
                Editor.MapOffset.x += extraOffset.x;
            if (!(Editor.MapOffset.y + extraOffset.y > maxOffset))
                Editor.MapOffset.y += extraOffset.y;
            Editor.GetMapCanvas().Invalidate();
        }

        public void HandlePressed(object sender, PointerRoutedEventArgs args)
        {
            Editor.IsMousePressing = true;
            Editor.SetCursor(CoreCursorType.Hand);
            _originPoint = GetMousePosition(args);
        }

        public void HandleReleased(object sender, PointerRoutedEventArgs args)
        {
            Editor.IsMousePressing = false;
            Editor.SetCursor(CoreCursorType.Arrow);
        }

        private Point GetMousePosition(PointerRoutedEventArgs args) =>
            args.GetCurrentPoint(Editor.GetMapCanvas()).Position;
    }

    /// <summary> 笔刷 </summary>
    [MapEditorToolHandler(Tool = MapEditorTool.Brush)]
    public class BrushHandler(MapEditor editor) : IMapEditorPointerEventsHandler
    {
        public MapEditor Editor { get; set; } = editor;

        public void HandleEntered(object sender, PointerRoutedEventArgs args)
        {
            Editor.SquareCursorManager.ShowCursor();
        }

        public void HandleExited(object sender, PointerRoutedEventArgs args)
        {
            Editor.SquareCursorManager.HideCursor();
            Editor.IsMousePressing = false;
        }

        public void HandleMoved(object sender, PointerRoutedEventArgs args)
        {
            if (!Editor.SquareCursorManager.IsVisible) return;

            // 更新笔刷光标
            var color = Editor.ViewModel.ClosestMapColor;
            Editor.SquareCursorManager.SetCursorColor(color);
            Editor.SquareCursorManager.SetCursorSize(Editor.MapScale);

            // 修改像素颜色
            if (!Editor.IsMousePressing) return;
            var mapPosition = GetMapPosition(this,args);
            if (Editor.IsMousePressing) PaintToMap(mapPosition.x, mapPosition.y, Editor.BrushRadius);
        }

        public void HandlePressed(object sender, PointerRoutedEventArgs args)
        {
            Editor.IsMousePressing = true;
            // 修改像素颜色
            var mapPosition = GetMapPosition(this,args);
            PaintToMap(mapPosition.x, mapPosition.y, Editor.BrushRadius);
        }

        /// <summary>
        ///     获取地图像素索引坐标
        /// </summary>
        private static (int x, int y) GetMapPosition(IMapEditorPointerEventsHandler handler,
            PointerRoutedEventArgs args)
        {
            return handler.GetMapPosition(args);
        }

        /// <summary>
        ///     以一个坐标为园心，向地图绘制颜色
        /// </summary>
        /// <remarks>当半径为 0 时，只绘制一个像素</remarks>
        private void PaintToMap(int pointX, int pointY, int radius)
        {
            if (radius != 0)
            {
                // 获取所有在圆内的点
                var validPoints = new List<(int x, int y)>();
                (int x, int y) recBegin = (pointX - radius, pointY + radius);
                (int x, int y) recEnd = (pointX + radius, pointY - radius);
                for (var y = recBegin.y; y >= recEnd.y; y--)
                {
                    for (var x = recBegin.x; x <= recEnd.x; x++)
                    {
                        if (Math.Sqrt(Math.Pow(x - pointX, 2) + Math.Pow(y - pointY, 2)) <= radius)
                            validPoints.Add((x, y));
                    }
                }

                // 绘制像素
                foreach (var validPoint in validPoints) PaintToMap(validPoint.x, validPoint.y, 0);
                return;
            }

            const int maxIndex = MapEditor.InitMapSize - 1;
            if (pointX > maxIndex || pointY > maxIndex || pointX < 0 || pointY < 0) return;
            SetColor(pointX, pointY, Editor.ViewModel.ClosestMapColor);
            Editor.GetMapCanvas().Invalidate();
            Editor.GetSaveButton().IsEnabled = true;
        }

        protected virtual void SetColor(int x, int y, Color color)
        {
            Editor.ViewModel.SetColor(x, y, color);
        }
    }
}