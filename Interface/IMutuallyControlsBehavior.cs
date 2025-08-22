using System;
using Microsoft.UI.Xaml;
using NBT_Studio.Utils;

namespace NBT_Studio.Interface;

public interface IMutuallyControlsBehavior
{
    IMutuallyControlsManager? MutuallyControlsManager { get; set; }
    string GroupName { get; set; }

    object Tag { get; set; }
    void UpdateState();

    /// <summary>
    ///     获取页面的 MutuallyControlsViewModel 类型的 MutuallyControlsManager
    /// </summary>
    static IMutuallyControlsManager GetManager(DependencyObject associatedObject)
    {
        if (VisualTreeHelper.GetSpecificDataContext<IMutuallyControlsManager>(associatedObject,
                out var managerViewModel))
            return managerViewModel;
        throw new InvalidOperationException(
            $"[{associatedObject.GetType()}] 使用了 [{nameof(IMutuallyControlsBehavior)}]，这要求页面实现 [{nameof(IMutuallyControlsManager)}] !");
    }

    void OnAssociatedObjectLoaded(object sender, RoutedEventArgs args)
    {
        MutuallyControlsManager = GetManager((DependencyObject)sender);
        MutuallyControlsManager.Register(this);
        UpdateState();
    }
}