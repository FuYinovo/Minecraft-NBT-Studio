using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Model;

namespace NBT_Studio.Xaml.Control.Popup;

public sealed partial class NotificationItem
{
    private InfoBarSeverity Severity { get; set; } = InfoBarSeverity.Informational;
    private string Title { get; set; } = string.Empty;
    private string Message { get; set; } = string.Empty;

    public NotificationItem()
    {
        InitializeComponent();
    }

    public void SetInfo(NotificationInfo info)
    {
        Title = info.Title;
        Message = info.Message;
        Severity = info.Severity;
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