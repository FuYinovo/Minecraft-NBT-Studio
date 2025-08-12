using System;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using NBT_Studio.Enum.Settings;
using NBT_Studio.Message;

namespace NBT_Studio;

public partial class App
{
    public static Window? MainWindow;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();
        MainWindow.Activate();
        RegisterMessages();
    }

    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<SettingsValueChangedMessage<Theme>>(this, (_, v) => SetTheme(v.Value));
    }

    private static void SetTheme(Theme theme)
    {
        if (MainWindow == null) return;
        ((FrameworkElement)MainWindow.Content).RequestedTheme = theme switch
        {
            Theme.Default => ElementTheme.Default,
            Theme.Dark => ElementTheme.Dark,
            Theme.Light => ElementTheme.Light,
            _ => throw new ArgumentOutOfRangeException(nameof(theme), theme, null)
        };
    }
}