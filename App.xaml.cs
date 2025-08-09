using Microsoft.UI.Xaml;

namespace NBT_Studio;

public partial class App
{
    public static readonly Window Window = new MainWindow();

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Window.Activate();
    }
}