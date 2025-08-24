using System.Windows.Input;
using Microsoft.UI.Xaml;

namespace NBT_Studio.Xaml.Control.MapEditorButton;

public partial class ButtonSeparate
{
    private readonly DependencyProperty _buttonCommandDp = DependencyProperty.Register(
        nameof(ButtonCommand),
        typeof(ICommand),
        typeof(ButtonSeparate),
        new PropertyMetadata(null));

    public ButtonSeparate()
    {
        InitializeComponent();
    }

    public ICommand ButtonCommand
    {
        get => (ICommand)GetValue(_buttonCommandDp);
        set => SetValue(_buttonCommandDp, value);
    }
}