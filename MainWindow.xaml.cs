using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.View;
using NBT_Studio.Xaml.Control.Popup;

namespace NBT_Studio;

public sealed partial class MainWindow
{
    public Grid NotificationGrid => BottomGrid;
    public MainWindow()
    {
        InitializeComponent();

        var presenter = OverlappedPresenter.Create();
        presenter.PreferredMinimumWidth = 1160;
        presenter.PreferredMinimumHeight = 765;
        AppWindow.SetPresenter(presenter);

        AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;

        Frame.Navigate(typeof(TreeViewPage));
    }
}