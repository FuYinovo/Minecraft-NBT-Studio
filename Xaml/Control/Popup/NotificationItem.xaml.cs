using System;
using Microsoft.UI.Xaml;
using NBT_Studio.Model;

namespace NBT_Studio.Xaml.Control.Popup;

public sealed partial class NotificationItem
{
    public NotificationItem(TimeSpan animationDuration)
    {
        InitializeComponent();
        AnimationDuration = animationDuration;
    }

    private Duration AnimationDuration { get; }

    public void SetInfo(NotificationInfo info)
    {
        InfoBar.Title = info.Title;
        InfoBar.Message = info.Message;
        InfoBar.Severity = info.Severity;
    }

    public void Show()
    {
        Popup.IsOpen = true;
        ShowingStoryboard.Begin();
    }

    public void Hide()
    {
        HidingStoryboard.Begin();
        HidingStoryboard.Completed += (_, _) => { Popup.IsOpen = false; };
    }
}