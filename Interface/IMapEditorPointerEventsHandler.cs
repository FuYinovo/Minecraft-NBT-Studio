using System;
using System.Numerics;
using Microsoft.UI.Xaml.Input;
using NBT_Studio.View;

namespace NBT_Studio.Interface;

public interface IMapEditorPointerEventsHandler
{
    MapEditor Editor { get; set; }

    void HandleEntered(object sender, PointerRoutedEventArgs args)
    {
    }

    void HandleExited(object sender, PointerRoutedEventArgs args)
    {
        Editor.IsMousePressing = false;
    }

    void HandleMoved(object sender, PointerRoutedEventArgs args)
    {
    }

    void HandlePressed(object sender, PointerRoutedEventArgs args)
    {
        Editor.IsMousePressing = true;
    }

    void HandleReleased(object sender, PointerRoutedEventArgs args)
    {
        Editor.IsMousePressing = false;
    }

    void WheelChanged(object sender, PointerRoutedEventArgs args)
    {
        // 获取滚轮 Delta 值
        var dWheel = args.GetCurrentPoint(Editor.GetMapCanvas()).Properties.MouseWheelDelta;
        // 计算当前缩放倍率
        var zoomFactor = Editor.GetMapScaleOrigin();
        // 应用新缩放倍率
        var newZoomFactor = zoomFactor + (float)dWheel / 24;
        if (newZoomFactor is < MapEditor.MinZoomFactor or > MapEditor.MaxZoomFactor) return;
        Editor.MapScale = newZoomFactor;
    }

    /// <summary>
    ///     获取地图像素索引坐标
    /// </summary>
    public (int x, int y) GetMapPosition(PointerRoutedEventArgs args)
    {
        Matrix3x2.Invert(Editor.MapTransform, out var inverted);
        var position = args.GetCurrentPoint(Editor.GetMapCanvas()).Position;
        var result = Vector2.Transform(new Vector2((float)position.X, (float)position.Y), inverted);
        var mapX = Math.Ceiling(result.X);
        var mapY = Math.Ceiling(result.Y);
        return ((int)mapX, (int)mapY);
    }

    /// <summary>
    ///     获取光标相对于地图画布位置
    /// </summary>
    public (float x, float y) GetCursorRelativePosition(PointerRoutedEventArgs args)
    {
        var position = args.GetCurrentPoint(Editor.GetMapCanvas()).Position;
        return ((float)position.X, (float)position.Y);
    }
}