using Microsoft.UI.Windowing;
using NBT_Studio.View;

namespace NBT_Studio;

public sealed partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        Frame.Navigate(typeof(TreeViewPage));
        AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;
    }
}