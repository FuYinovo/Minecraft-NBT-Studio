using NBT_Studio.ViewModel;

namespace NBT_Studio.Control;

public sealed partial class Settings
{
    private readonly SettingsViewModel _viewModel = new();

    public Settings()
    {
        InitializeComponent();
    }
}