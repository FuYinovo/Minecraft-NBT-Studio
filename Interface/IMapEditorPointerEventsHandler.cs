using System;
using System.Diagnostics;
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
        var wheelDelta = args.GetCurrentPoint(Editor.GetMapCanvas()).Properties.MouseWheelDelta;
        var zoomFactor = Math.Sqrt(Editor.MapScale) * 20 - 30;
        var newZoomFactor = (float)(zoomFactor + 0.03 * wheelDelta);
        if (newZoomFactor is < MapEditor.MinZoomFactor or > MapEditor.MaxZoomFactor) return;
        Editor.MapScale = newZoomFactor;
    }
}