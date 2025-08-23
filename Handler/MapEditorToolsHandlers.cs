using System;
using System.Diagnostics;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Core;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
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
    [MapEditorToolHandler(Tool = MapEditorTool.Eraser)]
    public class EraserHandler(MapEditor editor) : IMapEditorPointerEventsHandler
    {
        public MapEditor Editor { get; set; } = editor;
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
            var mapPosition = GetMapPosition(args);
            if (Editor.IsMousePressing) TryModifyMapColor(mapPosition.x, mapPosition.y);
        }

        public void HandlePressed(object sender, PointerRoutedEventArgs args)
        {
            Editor.IsMousePressing = true;
            // 修改像素颜色
            var mapPosition = GetMapPosition(args);
            TryModifyMapColor(mapPosition.x, mapPosition.y);
        }

        /// <summary>
        ///     获取地图像素索引坐标
        /// </summary>
        private (int x, int y) GetMapPosition(PointerRoutedEventArgs args)
        {
            var position = args.GetCurrentPoint(Editor.GetMapCanvas()).Position;
            var mapX = Math.Ceiling((position._x - Editor.MapOffset.x) / Editor.MapScale);
            var mapY = Math.Ceiling((position._y - Editor.MapOffset.y) / Editor.MapScale);
            return ((int)mapX, (int)mapY);
        }

        /// <summary>
        ///     尝试修改地图像素颜色
        /// </summary>
        /// <remarks>当传入地图坐标超出数组，直接返回</remarks>
        private void TryModifyMapColor(int x, int y)
        {
            const int maxIndex = MapEditor.InitMapSize - 1;
            if (x > maxIndex || y > maxIndex || x < 0 || y < 0) return;
            Editor.ViewModel.SetColor(x, y);
            Editor.GetMapCanvas().Invalidate();
            Editor.GetSaveButton().IsEnabled = true;
        }
    }
}