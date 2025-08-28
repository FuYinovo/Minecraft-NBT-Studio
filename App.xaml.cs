using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using NBT_Studio.Enum;
using NBT_Studio.Message;
using NBT_Studio.Service;
using NBT_Studio.Utils;
using UnhandledExceptionEventArgs = Microsoft.UI.Xaml.UnhandledExceptionEventArgs;

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
        SettingsService.GetInstance();
    }

    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<SettingsChangedMessage>(this, (_, msg) =>
        {
            if (msg.ValueType == typeof(Theme)) ApplyTheme((Theme)msg.NewValue);
        });
    }

    private static void ApplyTheme(Theme theme)
    {
        if (MainWindow == null) return;
        var elementTheme = theme switch
        {
            Theme.Dark => ElementTheme.Dark,
            Theme.Light => ElementTheme.Light,
            _ => ElementTheme.Default
        };
        var titleBarTheme = elementTheme switch
        {
            ElementTheme.Dark => TitleBarTheme.Dark,
            ElementTheme.Light => TitleBarTheme.Light,
            _ => TitleBarTheme.UseDefaultAppMode
        };
        ((FrameworkElement)MainWindow.Content).RequestedTheme = elementTheme;
        MainWindow.AppWindow.TitleBar.PreferredTheme = titleBarTheme;
    }
}