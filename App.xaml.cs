using Microsoft.UI.Xaml;

namespace NBT_Studio;

public partial class App
{

    public static Window? MainWindow ;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();
        MainWindow.Activate();
    }
}