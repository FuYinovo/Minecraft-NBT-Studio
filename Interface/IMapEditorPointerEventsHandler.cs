using System;
using System.Diagnostics;
using System.Numerics;
using Microsoft.UI.Xaml.Input;
using NBT_Studio.Enum;
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
        // 设置缩放中心到光标
        // var center = GetMapPosition(args);
        // Editor.ScaleCenter = new Vector2(center.x, center.y);
        // 计算当前缩放倍率
        var zoomFactor = Math.Sqrt(Editor.MapScale) * 20 - 30;
        // 应用新缩放倍率
        var newZoomFactor = (float)(zoomFactor + 0.03 * dWheel);
        if (newZoomFactor is < MapEditor.MinZoomFactor or > MapEditor.MaxZoomFactor) return;
        Editor.MapScale = newZoomFactor;

    }

    /// <summary>
    ///     获取地图像素索引坐标
    /// </summary>
    public (int x, int y) GetMapPosition(PointerRoutedEventArgs args)
    {
        var position = args.GetCurrentPoint(Editor.GetMapCanvas()).Position;
        var transform =
            Matrix3x2.CreateScale(Editor.MapScale, Editor.MapScale, Editor.ScaleCenter) *
            Matrix3x2.CreateTranslation(Editor.MapOffset.x, Editor.MapOffset.y);
        Matrix3x2.Invert(transform, out var inverted);

        var result = Vector2.Transform(new Vector2((float)position.X, (float)position.Y), inverted);
        var mapX = Math.Ceiling(result.X);
        var mapY = Math.Ceiling(result.Y);
        return ((int)mapX, (int)mapY);
    }
}