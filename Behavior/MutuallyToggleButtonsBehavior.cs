using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.Xaml.Interactivity;
using NBT_Studio.Interface;
using NBT_Studio.ViewModel;

namespace NBT_Studio.Behavior;

public class MutuallyToggleButtonsBehavior : Behavior<ToggleButton>, IMutuallyControlsBehavior
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Checked += OnChecked;
        AssociatedObject.Unchecked += OnUnchecked;
        AssociatedObject.Loaded += ((IMutuallyControlsBehavior)this).OnAssociatedObjectLoaded;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.Checked -= OnChecked;
        AssociatedObject.Unchecked -= OnUnchecked;
        ManagerViewModel?.Unregister(this);
        base.OnDetaching();
    }


    /// <summary>
    /// ToggleButton 选中时设置到 ManagerViewModel 中
    /// </summary>
    private void OnChecked(object sender, RoutedEventArgs routedEventArgs)
    {
        if (sender is not ToggleButton || ManagerViewModel is null) return;
        var selectedValue = ManagerViewModel.GetSelectedValue(GroupName);
        if (selectedValue != Value) ManagerViewModel.SetSelectedValue(GroupName, Value);
    }

    /// <summary>
    /// ToggleButton 取消选中时阻止
    /// </summary>
    private void OnUnchecked(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton toggleButton || ManagerViewModel is null) return;
        var selectedValue = ManagerViewModel.GetSelectedValue(GroupName);
        if (selectedValue == Value) toggleButton.IsChecked = true;
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

    public MutuallyControlsViewModel? ManagerViewModel { get; set; }
    public EventHandler<object>? OnBehaviorNoticed { get; set; }

    public void UpdateState()
    {
        if (ManagerViewModel is null) return;

        var currentValue = ManagerViewModel.GetSelectedValue(GroupName);
        AssociatedObject.IsChecked = Equals(currentValue, Value);
    }

    # endregion
}