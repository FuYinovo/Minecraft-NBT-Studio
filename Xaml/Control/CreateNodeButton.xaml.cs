using System.Windows.Input;
using Microsoft.UI.Xaml;
using NBT_Studio.Library.NBT_Parser.Enum;

namespace NBT_Studio.Xaml.Control;

public sealed partial class CreateNodeButton
{
    private readonly DependencyProperty _buttonCommandDp = DependencyProperty.Register(
        nameof(ButtonCommand), typeof(ICommand), typeof(CreateNodeButton), new PropertyMetadata(null));

    private readonly DependencyProperty _buttonEnabledDp = DependencyProperty.Register(
        nameof(ButtonEnabled), typeof(bool), typeof(CreateNodeButton), new PropertyMetadata(true));

    private readonly DependencyProperty _iconUriDp = DependencyProperty.Register(
        nameof(IconUri), typeof(string), typeof(CreateNodeButton), new PropertyMetadata(""));

    private readonly DependencyProperty _labelDp = DependencyProperty.Register(
        nameof(Label), typeof(string), typeof(CreateNodeButton), new PropertyMetadata(""));

    public CreateNodeButton()
    {
        InitializeComponent();
    }

    public string Label
    {
        get => (string)GetValue(_labelDp);
        set => SetValue(_labelDp, value);
    }

    public string IconUri
    {
        get => (string)GetValue(_iconUriDp);
        set => SetValue(_iconUriDp, value);
    }

    public ICommand? ButtonCommand
    {
        get => (ICommand)GetValue(_buttonCommandDp);
        set => SetValue(_buttonCommandDp, value);
    }

    public bool ButtonEnabled
    {
        get => (bool)GetValue(_buttonEnabledDp);
        set => SetValue(_buttonEnabledDp, value);
    }

    private void AppBarButton_OnClick(object sender, RoutedEventArgs e)
    {
        ButtonCommand?.Execute((NbtTagEnum)Tag);
    }
}