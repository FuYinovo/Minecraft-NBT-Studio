using System;
using System.Numerics;
using Windows.Foundation;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Graphics.Canvas.UI.Xaml;
using NBT_Studio.Message;
using NBT_Studio.ViewModel;

namespace NBT_Studio.Control.Editor;

public sealed partial class MapEditor
{
    private readonly MapEditorViewModel _viewModel = new(InitMapSize);
    private const double MapScaleFactor = 0.7;
    private const int InitMapSize = 128;
    private float _mapScale = 1;

    public MapEditor()
    {
        InitializeComponent();
        RegisterMessages();
    }

    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<DetailPadSizeChangedMessage>(this,
            (_, v) => UpdateGridSize(v.Value));
        WeakReferenceMessenger.Default.Register<DrawMapMessage>(this,
            (_, _) => MapCanvas.Invalidate());
    }

    // 自动调整内容大小
    private void UpdateGridSize(Size size)
    {
        if (size.Width <= 60) return;
        ControlGrid.Width = size.Width - 60;
        ControlGrid.Height = size.Height - 70;

        var mapSize = Math.Min(size.Width, size.Height) * MapScaleFactor;
        MapCanvas.Width = mapSize;
        MapCanvas.Height = mapSize;

        var borderSize = mapSize + 16;
        MapCanvasBorder.Width = borderSize;
        MapCanvasBorder.Height = borderSize;

        _mapScale = (float)(mapSize / InitMapSize);
        MapCanvas.Invalidate();
    }

    /// <summary>
    /// 绘制地图
    /// </summary>
    /// <remarks>Win2D</remarks>
    private void DrawMap(CanvasControl sender, CanvasDrawEventArgs args)
    {
        var session = args.DrawingSession;
        session.Transform = Matrix3x2.CreateScale(_mapScale, _mapScale);
        for (var y = 0; y < InitMapSize; y++)
        {
            for (var x = 0; x < InitMapSize; x++)
            {
                session.FillRectangle(x, y, 1, 1, _viewModel.Pixels[y, x].Color);
            }
        }
    }
}