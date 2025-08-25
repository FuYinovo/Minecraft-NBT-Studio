using Microsoft.UI.Xaml.Controls;

namespace NBT_Studio.Model;

public class NotificationInfo
{
    public string Title = string.Empty;
    public string Message = string.Empty;
    public InfoBarSeverity Severity = InfoBarSeverity.Informational;
}