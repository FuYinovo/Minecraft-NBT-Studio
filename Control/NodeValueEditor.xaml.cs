using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.ViewModel;

namespace NBT_Studio.Control;

public sealed partial class NodeValueEditor
{
    private readonly NodeValueEditorViewModel _viewModel = new();

    public NodeValueEditor()
    {
        InitializeComponent();
    }

    // 自动调整内容 Grid 大小 ( CommunityToolKit 未实现 )
    private void CommandBarSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is not TabbedCommandBar commandBar || commandBar.RenderSize.Width <= 60) return;
        ControlGrid.Width = commandBar.RenderSize.Width - 60;
        ControlGrid.Height = commandBar.RenderSize.Height - 70;
    }

    // 阻止 CheckBox 的改动
    private void CheckBox_Prohibit(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox checkBox) return;
        checkBox.IsChecked = !checkBox.IsChecked;
    }
}