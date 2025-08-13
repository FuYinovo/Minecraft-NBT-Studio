using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.ViewModel;

namespace NBT_Studio.Control;

public sealed partial class SettingsFlyout
{
    private readonly SettingsFlyoutViewModel _flyoutViewModel = new();

    public SettingsFlyout()
    {
        InitializeComponent();
        Loaded += UpdateBindings;
    }

    private void UpdateBindings(object sender, RoutedEventArgs e)
    {
        CombTheme.SelectedIndex = (int)_flyoutViewModel.Theme;
        CombSort.SelectedIndex = (int)_flyoutViewModel.Sort;
    }
}