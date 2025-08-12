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
    }

    private void Comb_Theme_OnLoaded(object sender, RoutedEventArgs e)
    {
       ((ComboBox)sender).SelectedIndex = (int)_flyoutViewModel.Theme;
    }

    private void Comb_Sort_OnLoaded(object sender, RoutedEventArgs e)
    {
        ((ComboBox)sender).SelectedIndex = (int)_flyoutViewModel.Sort;
    }
}