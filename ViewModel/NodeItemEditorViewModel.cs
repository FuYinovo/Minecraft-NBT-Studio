using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using NBT_Studio.Message;
using NBT_Studio.Model;

namespace NBT_Studio.ViewModel;

[SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator",
    "MVVMTK0045:Using [ObservableProperty] on fields is not AOT compatible for WinRT")]
public partial class NodeItemEditorViewModel : ObservableObject
{
    public NodeItemEditorViewModel()
    {
        RegisterMessages();
    }

    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<SelectedNodeChangedMessage>(this, (_, v) => { });
    }

    #region Properties

    [ObservableProperty] private List<MinecraftItem> _items = [];

    [ObservableProperty] private MinecraftItem _selectedItem = new()
    {
        Count = 0,
        Maximum = 64,
        Slot = 0,
        IdName = string.Empty,
        Name = string.Empty,
    };


    #endregion
}