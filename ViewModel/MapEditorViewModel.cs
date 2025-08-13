using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using NBT_Studio.Message;

namespace NBT_Studio.ViewModel;

[SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator",
    "MVVMTK0045:Using [ObservableProperty] on fields is not AOT compatible for WinRT")]
public partial class MapEditorViewModel : ObservableObject
{
    public MapEditorViewModel()
    {
        RegisterMessages();
    }

    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<SelectedNodeChangedMessage>(this, (_, v) => { });
    }
}