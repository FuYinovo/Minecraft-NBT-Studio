using Microsoft.UI.Xaml;

namespace NBT_Studio;

public partial class App
{
    public static Window? Window = new();

    public App()
    {
        InitializeComponent();
    }

    /// <summary>
    ///     Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Window = new MainWindow();
        Window.Activate();
    }
}