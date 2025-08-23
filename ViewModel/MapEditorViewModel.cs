using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Windows.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
public partial class MapEditorViewModel : ObservableObject, IMutuallyControlsManager
{
    public MapEditorViewModel(int mapSize)
    {
        _mapSize = mapSize;
        Pixels = new MinecraftMapPixel[_mapSize, _mapSize];
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
        Pixels = new MinecraftMapPixel[_mapSize, _mapSize];
        WeakReferenceMessenger.Default.Send(new DrawMapMessage());

        var nbtNode = (NbtNode)selectedTreeNode.Content;
        var dataTag = // 地图数据包含一个 data 字典
            nbtNode?.Tag.Children.Find(i => (i.Name ?? "").Equals("data", StringComparison.CurrentCultureIgnoreCase));
        if (dataTag is null) return;

        UpdateMapData(dataTag);
        UpdateColors();
        WeakReferenceMessenger.Default.Send(new DrawMapMessage());
    }

    /// <summary>
    ///     更新像素颜色
    /// </summary>
    private void UpdateColors()
    {
        if (!_dataTags.TryGetValue(MinecraftMapNecessaryTags.Colors, out var tag) || tag.Value is null) return;

        try
        {
            Pixels = new MinecraftMapPixel[_mapSize, _mapSize];
            var colors = (byte[])tag.Value;

            for (var y = 0; y < _mapSize; y++)
            for (var x = 0; x < _mapSize; x++)
            {
                var pixel = colors[y * _mapSize + x];
                var baseColor = (MinecraftMapColor)(byte)(pixel >> 2); // 前 6 位
                var modifyColor = (MinecraftColorModify)(byte)(pixel & 0b00000011); // 后 2 位
                var color = MinecraftMapColorExtensions.GetColor(baseColor, modifyColor);
                Pixels[y, x] = new MinecraftMapPixel { Color = color };
            }
        }
        catch (Exception)
        {
            // ignored
        }
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
    ///     将指定像素颜色设置为选定颜色(地图色)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetColor(int x, int y)
    {
        Pixels[y, x].Color = ClosestMapColor;
    }

    /// <summary>
    ///     设置互斥按钮组默认值
    /// </summary>
    private void SetDefaultButtonGroupsSelection()
    {
        ((IMutuallyControlsManager)this).SetValue("MapEditorTools", MapEditorTool.Select);
    }

    /// <summary>
    /// 保存地图修改
    /// </summary>
    [RelayCommand]
    private void SaveValueChanges()
    {
        // 将颜色转换为字节
        var colorBytes = new byte[Pixels.Length];
        var index = 0;
        foreach (var pixel in Pixels)
        {
            if (_colorToByte.TryGetValue(pixel.Color, out var colorByte)) colorBytes[index] = colorByte;
            else
            {
                var color = pixel.Color;
                _colorToByte[color] = MinecraftMapColorExtensions.GetColorByte(color);
                colorBytes[index] = _colorToByte[color];
            }

            index++;
        }

        // 保存到 NBT 标签
        _dataTags[MinecraftMapNecessaryTags.Colors].SetValue(colorBytes);
        // 通知修改消息
        WeakReferenceMessenger.Default.Send(new NodeModifiedMessage());
    }


    #region Properties

    private readonly Color[] _minecraftMapColors = MinecraftMapColorExtensions.GetAllColors(); // 所有地图色
    private readonly int _mapSize;
    private readonly Dictionary<MinecraftMapNecessaryTags, NbtTag> _dataTags = new();
    private readonly Dictionary<Color, byte> _colorToByte = new();

    public MapEditorTool SelectedTool => ((IMutuallyControlsManager)this).GetValue<MapEditorTool>("MapEditorTools");
    public Color ClosestMapColor => ColorHelper.GetClosest(BrushColor, _minecraftMapColors); // 选中色的最近地图色

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ClosestMapColor))]
    private Color _brushColor; // 选中色

    public MinecraftMapPixel[,] Pixels;

    #endregion Properties

    # region IMutuallyControlsManager

    public Dictionary<string, object> GroupToValue { get; } = new();
    public Dictionary<string, List<IMutuallyControlsBehavior>> RegisteredBehaviors { get; } = new();

    # endregion
}