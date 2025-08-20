using Microsoft.UI.Windowing;
using NBT_Studio.View;

namespace NBT_Studio;

public sealed partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        var presenter = OverlappedPresenter.Create();
        presenter.PreferredMinimumWidth = 1075;
        presenter.PreferredMinimumHeight = 755;
        AppWindow.SetPresenter(presenter);

        AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;

        Frame.Navigate(typeof(TreeViewPage));
    }
}