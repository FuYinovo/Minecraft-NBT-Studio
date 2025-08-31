using Windows.Graphics;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Utils;
using NBT_Studio.View;

namespace NBT_Studio;

public sealed partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        var presenter = OverlappedPresenter.Create();
        presenter.PreferredMinimumWidth = 856;
        presenter.PreferredMinimumHeight = 651;
        AppWindow.SetPresenter(presenter);
        AppWindow.SetIcon(AssetHelper.GetFilePathFromUri(AssetHelper.GetTitleBarIconUri()));
        AppWindow.Resize(new SizeInt32(1090, 760));

        AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

        Frame.Navigate(typeof(TreeViewPage));
    }

    public Grid NotificationGrid => BottomGrid;
}