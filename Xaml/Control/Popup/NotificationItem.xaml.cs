using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Model;

namespace NBT_Studio.Xaml.Control.Popup;

public sealed partial class NotificationItem
{
    private Duration AnimationDuration { get; }
    public NotificationItem(TimeSpan animationDuration)
    {
        InitializeComponent();
        AnimationDuration = animationDuration;
    }

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