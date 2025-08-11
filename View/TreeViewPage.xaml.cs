using NBT_Studio.ViewModel;

namespace NBT_Studio.View;

public sealed partial class TreeViewPage
{
    private readonly TreeViewPageViewModel _viewModel = new();

    public TreeViewPage()
    {
        InitializeComponent();
    }
}