using NBT_Studio.ViewModel;

namespace NBT_Studio.View;

public sealed partial class TreeViewPage
{
    private readonly TreeViewPageViewModel _viewModel = new() ;

    public TreeViewPage()
    {
        InitializeComponent();
        Settings.PageViewModel ??= _viewModel; // x:Bind 错误的赋值时机可能导致 null
    }
}