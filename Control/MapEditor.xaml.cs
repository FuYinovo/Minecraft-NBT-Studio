using Windows.Foundation;
using CommunityToolkit.Mvvm.Messaging;
using NBT_Studio.Message;
using NBT_Studio.ViewModel;

namespace NBT_Studio.Control;

public sealed partial class MapEditor
{
    private readonly MapEditorViewModel _viewModel = new();

    public MapEditor()
    {
        InitializeComponent();
        RegisterMessages();
    }

    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<NodeValuePanelSizeChangedMessage>(this,
            (_, v) => UpdateGridSize(v.Value));
    }

    // 自动调整内容 Grid 大小 ( CommunityToolKit 未实现 )
    private void UpdateGridSize(Size size)
    {
        if (size.Width <= 60) return;
        ControlGrid.Width = size.Width - 60;
        ControlGrid.Height = size.Height - 70;
    }
}