using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.Xaml.Interactivity;
using NBT_Studio.Interface;

namespace NBT_Studio.Xaml.Behavior;

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
        MutuallyControlsManager?.Unregister(this);
        base.OnDetaching();
    }


    /// <summary>
    ///     ToggleButton 选中时设置到 MutuallyControlsManager 中
    /// </summary>
    private void OnChecked(object sender, RoutedEventArgs routedEventArgs)
    {
        if (sender is not ToggleButton || MutuallyControlsManager is null) return;
        var selectedValue = MutuallyControlsManager.GetValue<object>(GroupName);
        if (selectedValue != Tag) MutuallyControlsManager.SetValue(GroupName, Tag);
    }

    /// <summary>
    ///     ToggleButton 取消选中时阻止
    /// </summary>
    private void OnUnchecked(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton toggleButton || MutuallyControlsManager is null) return;
        var selectedValue = MutuallyControlsManager.GetValue<object>(GroupName);
        if (selectedValue == Tag) toggleButton.IsChecked = true;
    }

    # region Dependency Properties

    private readonly DependencyProperty _groupNameDp = DependencyProperty.Register(
        nameof(GroupName),
        typeof(string),
        typeof(MutuallyToggleSplitButtonsBehavior),
        new PropertyMetadata(string.Empty));

    private readonly DependencyProperty _valueDp = DependencyProperty.Register(
        nameof(Tag),
        typeof(object),
        typeof(MutuallyToggleSplitButtonsBehavior),
        new PropertyMetadata(null));

    #endregion

    # region IMutuallyButtonBehavior

    public string GroupName
    {
        get => (string)GetValue(_groupNameDp);
        set => SetValue(_groupNameDp, value);
    }

    public object Tag
    {
        get => GetValue(_valueDp);
        set => SetValue(_valueDp, value);
    }

    public IMutuallyControlsManager? MutuallyControlsManager { get; set; }

    public void UpdateState()
    {
        if (MutuallyControlsManager is null) return;

        var currentValue = MutuallyControlsManager.GetValue<object>(GroupName);
        AssociatedObject.IsChecked = Equals(currentValue, Tag);
    }

    # endregion
}