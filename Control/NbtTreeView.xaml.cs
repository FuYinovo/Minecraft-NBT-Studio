using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.System;
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
    // 「插入节点」右键菜单
    private readonly List<NodeMenuFlyoutItem> _menuFlyoutItems =
    [
        new(NbtTagEnum.Byte, VirtualKey.B),
        new(NbtTagEnum.Short, VirtualKey.S),
        new(NbtTagEnum.Int, VirtualKey.I),
        new(NbtTagEnum.Long, VirtualKey.L),
        new(NbtTagEnum.Float, VirtualKey.F),
        new(NbtTagEnum.Double, VirtualKey.D),
        new(NbtTagEnum.String, VirtualKey.S),
        NodeMenuFlyoutItem.Separator,
        new(NbtTagEnum.ByteArray, VirtualKey.B, VirtualKeyModifiers.Control),
        new(NbtTagEnum.IntArray, VirtualKey.I, VirtualKeyModifiers.Control),
        new(NbtTagEnum.LongArray, VirtualKey.L, VirtualKeyModifiers.Control),
        NodeMenuFlyoutItem.Separator,
        new(NbtTagEnum.Dictionary, VirtualKey.D, VirtualKeyModifiers.Control),
        new(NbtTagEnum.List, VirtualKey.L, VirtualKeyModifiers.Control)
    ];

    public NbtTreeView()
    {
        InitializeComponent();
    }

    ///<summary> 展开或折叠子项 </summary>
    private void TreeViewItem_DoubleClicked(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (sender is not TreeViewItem treeViewItem) return;
        treeViewItem.IsExpanded = !treeViewItem.IsExpanded;
    }

    ///<summary> 通知「选择节点」修改的消息 </summary>
    private void SendSelectedNodeChangedMessage(TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        WeakReferenceMessenger.Default.Send(
            new SelectedNodeChangedMessage(sender.SelectedNode));
    }

    ///<summary> 动态生成「插入节点」右键菜单</summary>
    private void MenuFlyoutSubItem_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuFlyoutSubItem { DataContext: NbtNode node } menu) return;
        menu.Items.Clear();
        var command = node.AppendChildCommand; // Command
        foreach (var item in _menuFlyoutItems)
        {
            if (item.IsSeparator)
            {
                menu.Items.Add(new MenuFlyoutSeparator());
                continue;
            }

            menu.Items.Add(new MenuFlyoutItem
            {
                Text = item.Title,
                Command = command,
                CommandParameter = item.CommandParameter,
                KeyboardAccelerators =
                {
                    new KeyboardAccelerator
                    {
                        Key = item.Key,
                        Modifiers = item.ModifierKey
                    }
                }
            });
        }
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