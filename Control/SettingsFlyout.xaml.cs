using NBT_Studio.ViewModel;

namespace NBT_Studio.Control;

public sealed partial class SettingsFlyout
{
    private readonly SettingsFlyoutViewModel _flyoutViewModel = new();

    public SettingsFlyout()
    {
        InitializeComponent();
    }
}