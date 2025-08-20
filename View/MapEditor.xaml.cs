using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using NBT_Studio.Message;
using NBT_Studio.Service;
using NBT_Studio.ViewModel;

namespace NBT_Studio.View;

public sealed partial class MapEditor : INotifyPropertyChanged
{
    private const int InitMapSize = 128;
    private readonly MapEditorViewModel _viewModel = new(InitMapSize);
    private float _mapScale = 1;

    private float MapScale
    {
        get => _mapScale;
        set => _mapScale = (float)Math.Pow(0.05 * (value + 30), 2);
    }

    private readonly SquareCursorManager _cursorManager;

    public MapEditor()
    {
        InitializeComponent();
        RegisterMessages();
        HookCursorEvents();
        _cursorManager = new SquareCursorManager(MapCanvas);
        _cursorManager.HideCursor();
    }

    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<DetailPadSizeChangedMessage>(this,
            (_, v) => UpdateGridSize(v.Value));
        WeakReferenceMessenger.Default.Register<DrawMapMessage>(this,
            (_, _) => MapCanvas.Invalidate());
    }

    ///<summary> 自动调整内容大小 </summary>
    private void UpdateGridSize(Size size)
    {
        if (size.Width <= 60) return;
        ControlGrid.Width = size.Width - 60;
        ControlGrid.Height = size.Height - 70;
    }

    /// <summary>
    ///     绘制地图
    /// </summary>
    /// <remarks>Win2D</remarks>
    private void DrawMap(CanvasControl sender, CanvasDrawEventArgs args)
    {
        var session = args.DrawingSession;
        session.Transform = Matrix3x2.CreateScale(MapScale, MapScale);
        for (var y = 0; y < InitMapSize; y++)
        for (var x = 0; x < InitMapSize; x++)
            session.FillRectangle(x, y, 1, 1, _viewModel.Pixels[y, x].Color);
    }

    /// <summary>
    /// 绑定地图光标事件
    /// #
    /// </summary>
    private void HookCursorEvents()
    {
        MapCanvas.PointerEntered += (_, _) => { _cursorManager.ShowCursor(); };
        MapCanvas.PointerExited += (_, _) => { _cursorManager.HideCursor(); };
        MapCanvas.PointerMoved += (_, args) =>
        {
            if (!_cursorManager.IsVisible) return;
            var color = _viewModel.ClosestMapColor;
            var position = args.GetCurrentPoint(MapCanvas).Position;
            _cursorManager.SetCursorColor(color);
            _cursorManager.SetCursorSize(MapScale);
            _cursorManager.SetPosition(GetAdsorbPosition(position._x, position._y));
        };

        return;

        (float x, float y) GetAdsorbPosition(float x, float y)
        {
            return ((float, float))(Math.Floor(x / MapScale) * MapScale, Math.Floor(y / MapScale) * MapScale);
        }
    }

    # region INotifyPropertyChanged

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

    #endregion

    private void MapZoom_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        MapCanvas?.Invalidate();
    }
}