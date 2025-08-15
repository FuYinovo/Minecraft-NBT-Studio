using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using NBT_Studio.Interface;
using NBT_Studio.ViewModel;

namespace NBT_Studio.Behavior;

public class MutuallyToggleSplitButtonsBehavior : Behavior<ToggleSplitButton>, IMutuallyControlsBehavior
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.IsCheckedChanged += OnCheckedChanged;
        AssociatedObject.Loaded += ((IMutuallyControlsBehavior)this).OnAssociatedObjectLoaded;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.IsCheckedChanged -= OnCheckedChanged;
        ManagerViewModel?.Unregister(this);
        base.OnDetaching();
    }

    /// <summary>
    /// ToggleSplitButton 点击处理
    /// </summary>
    private void OnCheckedChanged(ToggleSplitButton sender, ToggleSplitButtonIsCheckedChangedEventArgs args)
    {
        if (ManagerViewModel is null) return;
        var selectedValue = ManagerViewModel.GetSelectedValue(GroupName);
        switch (sender.IsChecked)
        {
            // 尝试选中则设置到 ManagerViewModel
            case true:
                if (selectedValue != Value) ManagerViewModel.SetSelectedValue(GroupName, Value);
                break;
            // 尝试取消选中则阻止
            case false:
                if (selectedValue == Value) sender.IsChecked = false;
                break;
        }
    }


    # region Dependency Properties

    private readonly DependencyProperty _groupNameDependencyProperty = DependencyProperty.Register(
        nameof(GroupName),
        typeof(string),
        typeof(MutuallyToggleSplitButtonsBehavior),
        new PropertyMetadata(string.Empty));

    private readonly DependencyProperty _valueDependencyProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(object),
        typeof(MutuallyToggleSplitButtonsBehavior),
        new PropertyMetadata(null));

    #endregion

    # region IMutuallyButtonBehavior

    public string GroupName
    {
        get => (string)GetValue(_groupNameDependencyProperty);
        set => SetValue(_groupNameDependencyProperty, value);
    }

    public object Value
    {
        get => (string)GetValue(_valueDependencyProperty);
        set => SetValue(_valueDependencyProperty, value);
    }

    public void UpdateState()
    {
        if (ManagerViewModel is null) return;

        var currentValue = ManagerViewModel.GetSelectedValue(GroupName);
        AssociatedObject.IsChecked = Equals(currentValue, Value);
    }

    public MutuallyControlsViewModel? ManagerViewModel { get; set; }
    public EventHandler<object>? OnBehaviorNoticed { get; set; }

    # endregion
}