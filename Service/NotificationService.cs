using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NBT_Studio.Model;
using NBT_Studio.Xaml.Control.Popup;

namespace NBT_Studio.Service;

public class NotificationService
{
    private static readonly TimeSpan AnimationDuration = TimeSpan.FromSeconds(0.6);
    private static readonly TimeSpan NotificationDuration = TimeSpan.FromSeconds(2);
    private static readonly NotificationService Instance = new();
    private readonly NotificationItem _notification = new(AnimationDuration);
    private readonly Queue<NotificationInfo> _waitingQueue = [];
    private bool _isShowing;

    private NotificationService()
    {
        if (App.MainWindow is null) return;
        App.MainWindow.NotificationGrid.Children.Add(_notification);
    }

    public static NotificationService GetInstance() => Instance;

    public async Task Send(NotificationInfo info)
    {
        if (_isShowing)
        {
            _waitingQueue.Enqueue(info);
            return;
        }

        await ShowNotification(info);

        if (_waitingQueue.Count > 0)
            await Send(_waitingQueue.Dequeue());
    }

    private async Task ShowNotification(NotificationInfo info)
    {
        _isShowing = true;
        _notification.SetInfo(info);
        _notification.Show();
        await Task.Delay(AnimationDuration + NotificationDuration);

        _isShowing = false;
        _notification.Hide();
        await Task.Delay(AnimationDuration);
    }
}