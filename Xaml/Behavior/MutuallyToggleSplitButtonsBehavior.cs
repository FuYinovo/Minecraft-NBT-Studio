using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using NBT_Studio.Interface;

namespace NBT_Studio.Xaml.Behavior;

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
        MutuallyControlsManager?.Unregister(this);
        base.OnDetaching();
    }

    /// <summary>
    ///     ToggleSplitButton 点击处理
    /// </summary>
    private void OnCheckedChanged(ToggleSplitButton sender, ToggleSplitButtonIsCheckedChangedEventArgs args)
    {
        if (MutuallyControlsManager is null) return;
        var selectedValue = MutuallyControlsManager.GetSelectedValue(GroupName);
        switch (sender.IsChecked)
        {
            // 尝试选中则设置到 MutuallyControlsManager
            case true:
                if (selectedValue != Tag) MutuallyControlsManager.SetSelectedValue(GroupName, Tag);
                break;
            // 尝试取消选中则阻止
            case false:
                if (selectedValue == Tag) sender.IsChecked = false;
                break;
        }
    }


    # region Dependency Properties

    private readonly DependencyProperty _groupNameDependencyProperty = DependencyProperty.Register(
        nameof(GroupName),
        typeof(string),
        typeof(MutuallyToggleSplitButtonsBehavior),
        new PropertyMetadata(string.Empty));

    private readonly DependencyProperty _tagDependencyProperty = DependencyProperty.Register(
        nameof(Tag),
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

    public object Tag
    {
        get => GetValue(_tagDependencyProperty);
        set => SetValue(_tagDependencyProperty, value);
    }

    public void UpdateState()
    {
        if (MutuallyControlsManager is null) return;

        var currentValue = MutuallyControlsManager.GetSelectedValue(GroupName);
        AssociatedObject.IsChecked = Equals(currentValue, Tag);
    }

    public IMutuallyControlsManager? MutuallyControlsManager { get; set; }

    # endregion
}