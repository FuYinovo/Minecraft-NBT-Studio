using Microsoft.UI.Xaml;
using NBT_Studio.ViewModel;

namespace NBT_Studio.Control;

public sealed partial class Settings
{
    public Settings()
    {
        InitializeComponent();
        if (PageViewModel != null) _viewModel = new SettingsViewModel(PageViewModel);
    }

    #region Properties

    // 若 PageViewModel 为 null，则不实例化 SettingsViewModel
    private SettingsViewModel? _viewModel;

    // x:Bind 错误的赋值时机可能导致 null
    public TreeViewPageViewModel? PageViewModel
    {
        get => (TreeViewPageViewModel)GetValue(_pageViewModelProperty);
        set
        {
            SetValue(_pageViewModelProperty, value);
            if (value != null) _viewModel = new SettingsViewModel(value);
        }
    }

    private readonly DependencyProperty _pageViewModelProperty =
        DependencyProperty.Register(
            nameof(PageViewModel),
            typeof(TreeViewPageViewModel),
            typeof(Settings),
            new PropertyMetadata(null));

    #endregion
}