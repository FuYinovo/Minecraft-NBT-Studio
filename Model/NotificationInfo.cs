using Microsoft.UI.Xaml.Controls;

namespace NBT_Studio.Model;

public class NotificationInfo(string title, string message, InfoBarSeverity severity = InfoBarSeverity.Informational)
{
    public string Title = title;
    public string Message = message;
    public InfoBarSeverity Severity = severity;
}