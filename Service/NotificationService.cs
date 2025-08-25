using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using NBT_Studio.Model;
using NBT_Studio.Xaml.Control.Popup;
using Vanara.PInvoke;

namespace NBT_Studio.Service;

public class NotificationService
{
    private static readonly NotificationService Instance = new();
    public static NotificationService GetInstance() => Instance;
    private readonly NotificationItem _notification = new();
    private readonly List<NotificationInfo> _waitingList = [];
    private bool _isShowing;

    private NotificationService()
    {
        if (App.MainWindow is null) return;
        App.MainWindow.NotificationGrid.Children.Add(_notification);
    }

    public async Task Send(NotificationInfo info)
    {
        if (_isShowing)
        {
            _waitingList.Add(info);
            return;
        }

        _notification.SetInfo(info);
        _notification.Show();
        _isShowing = true;
        await Task.Delay(TimeSpan.FromSeconds(0.6 + 2)); // 动画时长 0.6s
        _notification.Hide();
        await Task.Delay(TimeSpan.FromSeconds(0.6));
        _isShowing = false;

        if (_waitingList.Contains(info)) _waitingList.Remove(info);

        if (_waitingList.Count > 0)
            await Send(_waitingList.First());
    }
}