using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NBT_Studio.Xaml.Control.MapEditorButton;

public class ButtonBase : UserControl
{
    private readonly DependencyProperty _fontIconDp = DependencyProperty.Register(
        nameof(FontIcon),
        typeof(string),
        typeof(ToggleButtonMutually),
        new PropertyMetadata(null)
    );

    private readonly DependencyProperty _groupNameDp = DependencyProperty.Register(
        nameof(GroupName),
        typeof(string),
        typeof(ToggleButtonMutually),
        new PropertyMetadata(string.Empty)
    );

    private readonly DependencyProperty _memberTagDp = DependencyProperty.Register(
        nameof(MemberTag),
        typeof(object),
        typeof(ToggleButtonMutually),
        new PropertyMetadata(string.Empty)
    );

    private readonly DependencyProperty _titleDp = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(ToggleButtonMutually),
        new PropertyMetadata(string.Empty)
    );

    public string FontIcon
    {
        get => (string)GetValue(_fontIconDp);
        set => SetValue(_fontIconDp, value);
    }

    public string GroupName
    {
        get => (string)GetValue(_groupNameDp);
        set => SetValue(_groupNameDp, value);
    }

    public object MemberTag
    {
        get => GetValue(_memberTagDp);
        set => SetValue(_memberTagDp, value);
    }

    public string Title
    {
        get => (string)GetValue(_titleDp);
        set => SetValue(_titleDp, value);
    }
}