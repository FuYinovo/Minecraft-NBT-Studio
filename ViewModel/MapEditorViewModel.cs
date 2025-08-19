using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Windows.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Enum;
using NBT_Studio.Interface;
using NBT_Studio.Library.NBT_Parser.Class;
using NBT_Studio.Message;
using NBT_Studio.Model;
using ColorHelper = NBT_Studio.Utils.ColorHelper;

namespace NBT_Studio.ViewModel;

[SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator",
    "MVVMTK0045:Using [ObservableProperty] on fields is not AOT compatible for WinRT")]
public partial class MapEditorViewModel : ObservableObject, IMutuallyControls
{
    public MapEditorViewModel(int size)
    {
        _size = size;
        Pixels = new MinecraftMapPixel[_size, _size];
        RegisterMessages();
        SetDefaultButtonGroupsSelection();
    }

    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<SelectedNodeChangedMessage>(this,
            (_, v) => UpdateMapDisplay(v.Value));
    }

    /// <summary>
    ///     更新地图显示
    /// </summary>
    /// <param name="selectedTreeNode"></param>
    private void UpdateMapDisplay(TreeViewNode selectedTreeNode)
    {
        var nbtNode = (NbtNode)selectedTreeNode.Content;
        var dataTag = // 地图数据包含一个 data 字典
            nbtNode?.Tag.Children.Find(i => (i.Name ?? "").Equals("data", StringComparison.CurrentCultureIgnoreCase));
        if (dataTag is null) return;

        UpdateMapData(dataTag);
        UpdateColors();
    }

    /// <summary>
    ///     更新像素颜色
    /// </summary>
    private void UpdateColors()
    {
        if (!_dataTags.TryGetValue(MinecraftMapNecessaryTags.Colors, out var tag) || tag.Value is null) return;

        try
        {
            Pixels = new MinecraftMapPixel[_size, _size];
            var colors = (byte[])tag.Value;

            for (var y = 0; y < _size; y++)
            for (var x = 0; x < _size; x++)
            {
                var pixel = colors[y * _size + x];
                var baseColor = (MinecraftMapColor)(byte)(pixel >> 2); // 前 6 位
                var modifyColor = (MinecraftColorModify)(byte)(pixel & 0b00000011); // 后 2 位
                var color = MinecraftMapColorExtensions.GetColor(baseColor, modifyColor);
                Pixels[y, x] = new MinecraftMapPixel { Color = color };
            }
        }
        catch (Exception)
        {
            return;
        }

        WeakReferenceMessenger.Default.Send(new DrawMapMessage());
    }

    /// <summary>
    ///     从节点更新地图数据
    /// </summary>
    private void UpdateMapData(NbtTag dataTag)
    {
        _dataTags.Clear();
        foreach (var child in dataTag.Children)
            if (System.Enum.TryParse(child.Name ?? string.Empty, true, out MinecraftMapNecessaryTags tagEnum))
                _dataTags[tagEnum] = child;
    }

    /// <summary>
    ///     设置互斥按钮组默认值
    /// </summary>
    private void SetDefaultButtonGroupsSelection()
    {
        ((IMutuallyControls)this).SetSelectedValue("MapEditorTools", "All");
    }


    #region Properties

    private readonly Color[] _minecraftMapColors = MinecraftMapColorExtensions.GetAllColors(); // 所有地图色
    [ObservableProperty] private Color _brushColor; // 选中色
    public Color ClosestMapColor => ColorHelper.GetClosest(BrushColor, _minecraftMapColors); // 选中色的最近地图色
    public MinecraftMapPixel[,] Pixels;
    private readonly int _size;
    private readonly Dictionary<MinecraftMapNecessaryTags, NbtTag> _dataTags = new();

    #endregion Properties

    # region IMutuallyControls

    public Dictionary<string, object> GroupToValue { get; } = new();
    public Dictionary<string, List<IMutuallyControlsBehavior>> RegisteredBehaviors { get; } = new();

    # endregion
}