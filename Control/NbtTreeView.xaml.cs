using System.Collections.ObjectModel;
using System.Linq;
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
    private readonly NodeMenuFlyoutItem[] _menuFlyoutItems;

    public NbtTreeView()
    {
        _menuFlyoutItems = GetMenuFlyoutItems();
        InitializeComponent();
    }

    private static NodeMenuFlyoutItem[] GetMenuFlyoutItems()
    {
        var numbers = NbtTagEnumExtensions.Numbers
            .Select(x => new NodeMenuFlyoutItem(x, GetKey(x)));
        var others = NbtTagEnumExtensions.Others
            .Select(x => new NodeMenuFlyoutItem(x, GetKey(x)));
        var arrays = NbtTagEnumExtensions.Arrays
            .Select(x =>
                new NodeMenuFlyoutItem(x, GetKey(x), VirtualKeyModifiers.Control));
        var collections = NbtTagEnumExtensions.Collections
            .Select(x =>
                new NodeMenuFlyoutItem(x, GetKey(x), VirtualKeyModifiers.Control));
        return
        [
            ..numbers,
            ..others,
            NodeMenuFlyoutItem.Separator,
            ..arrays,
            NodeMenuFlyoutItem.Separator,
            ..collections
        ];

        VirtualKey GetKey(NbtTagEnum tag)
        {
            return System.Enum.TryParse(tag.ToString().ToUpper()[..1], out VirtualKey key)
                ? key
                : VirtualKey.None;
        }
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

        // 如果不是容器节点，则禁用该右键菜单
        if (!NbtTagEnumExtensions.IsCollection(node.TagEnum))
        {
            menu.IsEnabled = false;
            return;
        }

        if (menu.Items.Count == _menuFlyoutItems.Length) return; // 已经添加则返回
        var command = node.AppendChildCommand; // 右键菜单的 Command
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
                // 对于列表节点，禁用与其子项类型不同的添加选项
                IsEnabled = node.TagEnum != NbtTagEnum.List || item.CommandParameter == node.Tag.ChildrenTag,
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