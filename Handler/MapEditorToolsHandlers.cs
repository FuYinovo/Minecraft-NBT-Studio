using System;
using Microsoft.UI.Xaml.Input;
using NBT_Studio.Attribute;
using NBT_Studio.Enum;
using NBT_Studio.Interface;
using NBT_Studio.View;

namespace NBT_Studio.Handler;

public record MapEditorToolsHandlers
{
    /// <summary> 选择 </summary>
    [MapEditorToolHandler(Tool = MapEditorTool.Select)]
    public class SelectHandler(MapEditor editor) : IPointerEventsHandler
    {
        public MapEditor Editor { get; set; } = editor;
    }

    /// <summary> 橡皮 </summary>
    [MapEditorToolHandler(Tool = MapEditorTool.Eraser)]
    public class EraserHandler(MapEditor editor) : IPointerEventsHandler
    {
        public MapEditor Editor { get; set; } = editor;
    }

    /// <summary> 吸色器 </summary>
    [MapEditorToolHandler(Tool = MapEditorTool.ColorPicker)]
    public class ColorPickerHandler(MapEditor editor) : IPointerEventsHandler
    {
        public MapEditor Editor { get; set; } = editor;
    }

    /// <summary> 缩放 </summary>
    [MapEditorToolHandler(Tool = MapEditorTool.Zoom)]
    public class ZoomHandler(MapEditor editor) : IPointerEventsHandler
    {
        public MapEditor Editor { get; set; } = editor;
    }

    /// <summary> 笔刷 </summary>
    [MapEditorToolHandler(Tool = MapEditorTool.Brush)]
    public class BrushHandler(MapEditor editor) : IPointerEventsHandler
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
            var position = args.GetCurrentPoint(Editor.MapCanvasControl).Position;
            var mapX = Math.Ceiling(position._x / Editor.MapScale);
            var mapY = Math.Ceiling(position._y / Editor.MapScale);
            return ((int)mapX, (int)mapY);
        }

        /// <summary>
        ///     尝试修改地图像素颜色
        /// </summary>
        /// <remarks>当传入地图坐标超出数组，直接返回</remarks>
        private void TryModifyMapColor(int x, int y)
        {
            var maxIndex = Editor.ViewModel.Pixels.Length;
            if (x > maxIndex || y > maxIndex) return;
            Editor.ViewModel.SetColor(x, y);
            Editor.MapCanvasControl.Invalidate();
            Editor.SaveButtonControl.IsEnabled = true;
        }
    }
}