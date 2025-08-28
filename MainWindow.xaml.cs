using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Utils;
using NBT_Studio.View;
using NBT_Studio.Xaml.Control.Popup;
using Microsoft.UI;

namespace NBT_Studio;

public sealed partial class MainWindow
{
    public Grid NotificationGrid => BottomGrid;
    public MainWindow()
    {
        InitializeComponent();

        var presenter = OverlappedPresenter.Create();
        presenter.PreferredMinimumWidth = 856;
        presenter.PreferredMinimumHeight = 651;
        AppWindow.SetPresenter(presenter);

        AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

        Frame.Navigate(typeof(TreeViewPage));
    }
}