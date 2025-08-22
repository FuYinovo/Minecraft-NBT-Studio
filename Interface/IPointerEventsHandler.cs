using Microsoft.UI.Xaml.Input;
using NBT_Studio.View;

namespace NBT_Studio.Interface;

public interface IPointerEventsHandler
{
    MapEditor Editor { get; set; }

    void HandleEntered(object sender, PointerRoutedEventArgs args)
    {
    }

    void HandleExited(object sender, PointerRoutedEventArgs args)
    {
    }

    void HandleMoved(object sender, PointerRoutedEventArgs args)
    {
        Editor.IsMousePressing = false;
    }

    void HandlePressed(object sender, PointerRoutedEventArgs args)
    {
        Editor.IsMousePressing = true;
    }

    void HandleReleased(object sender, PointerRoutedEventArgs args)
    {
        Editor.IsMousePressing = false;
    }
}