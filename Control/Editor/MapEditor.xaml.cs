using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Graphics.Canvas.UI.Xaml;
using NBT_Studio.Message;
using NBT_Studio.ViewModel;

namespace NBT_Studio.Control.Editor;

public sealed partial class MapEditor : INotifyPropertyChanged
{
    private readonly MapEditorViewModel _viewModel = new(InitMapSize);
    private const double MapScaleFactor = 0.7;
    private const int InitMapSize = 128;
    private float _mapScale = 1;


    # region UI Observable Properties

    private double _mapSize = 256.0;
    public double BorderSize => MapSize + 16;

    private double MapSize
    {
        get => _mapSize;
        set
        {
            SetField(ref _mapSize, value);
            OnPropertyChanged(nameof(BorderSize));
        }
    }

    # endregion

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

        MapSize = Math.Min(size.Width, size.Height) * MapScaleFactor;
        _mapScale = (float)(MapSize / InitMapSize);
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

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(propertyName);
    }
}