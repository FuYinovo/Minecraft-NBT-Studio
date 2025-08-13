using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using NBT_Studio.Library.NBT_Parser.Enum;
using NBT_Studio.Message;
using NBT_Studio.Model;

namespace NBT_Studio.Control;

public sealed partial class NbtTreeView
{
    public NbtTreeView()
    {
        InitializeComponent();
    }

    /// <summary>
    ///     展开或折叠子项
    /// </summary>
    private void TreeViewItem_DoubleClicked(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (sender is not TreeViewItem treeViewItem) return;
        treeViewItem.IsExpanded = !treeViewItem.IsExpanded;
    }

    /// <summary>
    ///     通知「选择节点」修改的消息
    /// </summary>
    private void SendSelectedNodeChangedMessage(TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        WeakReferenceMessenger.Default.Send(
            new SelectedNodeChangedMessage(sender.SelectedNode));
    }

    #region Properties

    private readonly DependencyProperty _nodesDependencyProperty = DependencyProperty.Register(
        nameof(Nodes),
        typeof(ObservableCollection<NbtNode>),
        typeof(NbtTreeView),
        new PropertyMetadata(new ObservableCollection<NbtNode>()));

    public ObservableCollection<NbtNode> Nodes
    {
        get => (ObservableCollection<NbtNode>)GetValue(_nodesDependencyProperty);
        set => SetValue(_nodesDependencyProperty, value);
    }

    #endregion
}