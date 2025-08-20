using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NBT_Studio.Xaml.Control.MapEditorToggleButton;

public class MapEditorToggleButtonBase : UserControl
{
    private readonly DependencyProperty _fontIconDp = DependencyProperty.Register(
        nameof(FontIcon),
        typeof(string),
        typeof(MapEditorToggleButton),
        new PropertyMetadata(null)
    );

    private readonly DependencyProperty _groupNameDp = DependencyProperty.Register(
        nameof(GroupName),
        typeof(string),
        typeof(MapEditorToggleButton),
        new PropertyMetadata(null)
    );

    private readonly DependencyProperty _memberNameDp = DependencyProperty.Register(
        nameof(MemberName),
        typeof(string),
        typeof(MapEditorToggleButton),
        new PropertyMetadata(null)
    );

    private readonly DependencyProperty _titleDp = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(MapEditorToggleButton),
        new PropertyMetadata(null)
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

    public string MemberName
    {
        get => (string)GetValue(_memberNameDp);
        set => SetValue(_memberNameDp, value);
    }

    public string Title
    {
        get => (string)GetValue(_titleDp);
        set => SetValue(_titleDp, value);
    }
}