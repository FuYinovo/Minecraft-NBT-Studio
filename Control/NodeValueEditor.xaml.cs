using System.Diagnostics;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Library.NBT_Parser.Enum;
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

    // 禁止信息控件的改动
    private void CheckBox_Prohibit(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox checkBox) return;
        Utils.ControlDisabler.DisableCheckBox(checkBox);
    }

    private void ComboBox_Type_Prohibit(object? sender, object e)
    {
        if (sender is not ComboBox comboBox) return;
        Utils.ControlDisabler.DisableComboBoxItemsByTag(comboBox, _viewModel.TagEnum);
    }

    private void ComboBox_ChildrenType_Prohibit(object? sender, object e)
    {
        if (sender is not ComboBox comboBox) return;
        Utils.ControlDisabler.DisableComboBoxItemsByTag(comboBox, _viewModel.ChildrenTagEnum);
    }
}