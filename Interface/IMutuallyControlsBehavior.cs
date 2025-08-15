using System;
using Microsoft.UI.Xaml;
using NBT_Studio.Utils;
using NBT_Studio.ViewModel;

namespace NBT_Studio.Interface;

public interface IMutuallyControlsBehavior
{
    MutuallyControlsViewModel? ManagerViewModel { get; set; }
    string GroupName { get; set; }

    object Value { get; set; }
    void UpdateState();

    /// <summary>
    /// 获取页面的 MutuallyControlsViewModel 类型的 ManagerViewModel
    /// </summary>
    static MutuallyControlsViewModel GetViewModel(DependencyObject associatedObject)
    {
        if (TreeHelper.GetViewModel<MutuallyControlsViewModel>(associatedObject, out var managerViewModel))
            return managerViewModel;
        throw new InvalidOperationException(
            $"[{associatedObject.GetType()}] 使用了 [{nameof(IMutuallyControlsBehavior)}]，这要求页面继承 [{nameof(MutuallyControlsViewModel)}] !");
    }

    void OnAssociatedObjectLoaded(object sender, RoutedEventArgs args)
    {
        ManagerViewModel = GetViewModel((DependencyObject)sender);
        ManagerViewModel.Register(this);
        UpdateState();
    }
}