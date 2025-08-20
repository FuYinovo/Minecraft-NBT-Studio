using System;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using NBT_Studio.Enum.Settings;
using NBT_Studio.Message;

namespace NBT_Studio;

public partial class App
{
    public static MainWindow? MainWindow;

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
        WeakReferenceMessenger.Default.Register<SettingsChangedMessage<Theme>>(this, (_, v) => SetTheme(v.Value));
    }

    private static void SetTheme(Theme theme)
    {
        if (MainWindow == null) return;
        ((FrameworkElement)MainWindow.Content).RequestedTheme = ThemeExtensions.ToElementTheme(theme);
        MainWindow.AppWindow.TitleBar.PreferredTheme = ThemeExtensions.ToTitleBarTheme(theme);
    }
}