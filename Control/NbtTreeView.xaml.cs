using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using NBT_Studio.Model;


namespace NBT_Studio.Control;

public sealed partial class NbtTreeView
{
    private const string AddIcon = "\uF8AA";
    private const string SubtractIcon = "\uF8AB";
    private Dictionary<int, Button> hashCodeToButton = new();
    public ObservableCollection<NbtNode> Nodes
    {
        get => (ObservableCollection<NbtNode>)GetValue(_nodesDependencyProperty);
        set => SetValue(_nodesDependencyProperty, value);
    }

    private readonly DependencyProperty _nodesDependencyProperty = DependencyProperty.Register(
        nameof(Nodes),
        typeof(ObservableCollection<NbtNode>),
        typeof(NbtTreeView),
        new PropertyMetadata(new ObservableCollection<NbtNode>()));


    public NbtTreeView()
    {
        InitializeComponent();
    }


    /// <summary>
    /// 查找一个控件的最近父控件
    /// </summary>
    /// <exception cref="InvalidOperationException">查找失败</exception>
    private static T GetParent<T>(DependencyObject child) where T : class
    {
        var parent = VisualTreeHelper.GetParent(child);
        while (parent != null && parent is not T)
        {
            parent = VisualTreeHelper.GetParent(parent);
        }

        return parent as T ?? throw new InvalidOperationException($"查找{child.GetType()}的最近父类{typeof(T)}失败");
    }

    /// <summary>
    ///    展开或折叠子项
    /// </summary>
    private void TreeViewItem_DoubleClicked(object sender, DoubleTappedRoutedEventArgs e)
    {
        if(sender is not TreeViewItem treeViewItem) return;
        treeViewItem.IsExpanded = !treeViewItem.IsExpanded;
    }
}







// /// <summary>
// /// 对 TreeViewItem 的 IsExpanded 取反、修改图标
// /// </summary>
// private void ExpandButton_OnClick(object sender, RoutedEventArgs e)
// {
//     if (sender is not Button button) return;
//     // 取反 IsExpanded
//     var treeViewItem = GetParent<TreeViewItem>(button);
//     treeViewItem.IsExpanded = !treeViewItem.IsExpanded;
//     // 修改图标
//     var fontIcon = (FontIcon)((Border)button.ContentTemplateRoot).Child;
//     var glyph = fontIcon.Glyph;
//     fontIcon.Glyph = glyph == AddIcon ? SubtractIcon : AddIcon;
// }