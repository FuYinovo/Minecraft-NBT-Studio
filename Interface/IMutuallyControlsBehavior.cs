using System;
using Microsoft.UI.Xaml;
using NBT_Studio.Utils;

namespace NBT_Studio.Interface;

public interface IMutuallyControlsBehavior
{
    IMutuallyControls? MutuallyControlsManager { get; set; }
    string GroupName { get; set; }

    object Value { get; set; }
    void UpdateState();

    /// <summary>
    ///     获取页面的 MutuallyControlsViewModel 类型的 MutuallyControlsManager
    /// </summary>
    static IMutuallyControls GetManager(DependencyObject associatedObject)
    {
        if (TreeHelper.GetSpecificDataContext<IMutuallyControls>(associatedObject, out var managerViewModel))
            return managerViewModel;
        throw new InvalidOperationException(
            $"[{associatedObject.GetType()}] 使用了 [{nameof(IMutuallyControlsBehavior)}]，这要求页面实现 [{nameof(IMutuallyControls)}] !");
    }

    void OnAssociatedObjectLoaded(object sender, RoutedEventArgs args)
    {
        MutuallyControlsManager = GetManager((DependencyObject)sender);
        MutuallyControlsManager.Register(this);
        UpdateState();
    }
}