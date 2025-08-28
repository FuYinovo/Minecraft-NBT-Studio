using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using NBT_Studio.Message;
using NBT_Studio.ViewModel;

namespace NBT_Studio.View;

public sealed partial class TreeViewPage
{
    private readonly TreeViewPageViewModel _viewModel = new();

    public TreeViewPage()
    {
        InitializeComponent();
    }

    // 发送「节点详情区域大小更新」信息
    private void DetailSectionSizeChanged(object sender, SizeChangedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new DetailPadSizeChangedMessage(e.NewSize));
    }
}