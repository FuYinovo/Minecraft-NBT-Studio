using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace NBT_Studio.Control;

public sealed partial class NbtInsertButton
{
    public string Label
    {
        get => (string)GetValue(_labelDependencyProperty);
        set => SetValue(_labelDependencyProperty, value);
    }

    public string IconUri
    {
        get => (string)GetValue(_iconUriDependencyProperty);
        set => SetValue(_iconUriDependencyProperty, value);
    }

    public ICommand ButtonCommand
    {
        get => (ICommand)GetValue(_buttonCommandDependencyProperty);
        set => SetValue(_buttonCommandDependencyProperty, value);
    }

    private readonly DependencyProperty _labelDependencyProperty = DependencyProperty.Register(
        nameof(Label), typeof(string), typeof(NbtInsertButton), new PropertyMetadata(""));

    private readonly DependencyProperty _iconUriDependencyProperty = DependencyProperty.Register(
        nameof(IconUri), typeof(string), typeof(NbtInsertButton), new PropertyMetadata(""));

    private readonly DependencyProperty _buttonCommandDependencyProperty = DependencyProperty.Register(
        nameof(ButtonCommand), typeof(ICommand), typeof(NbtInsertButton), new PropertyMetadata(null));

    public NbtInsertButton()
    {
        InitializeComponent();
    }
}