using System;
using System.Numerics;
using Windows.Foundation;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using NBT_Studio.Manager;
using NBT_Studio.Message;
using NBT_Studio.ViewModel;

namespace NBT_Studio.View;

public sealed partial class MapEditor
{
    private const int InitMapSize = 128;

    private readonly SquareCursorManager _cursorManager;
    private readonly MapEditorViewModel _viewModel = new(InitMapSize);
    private bool _isMousePressing;
    private float _mapScale = 1;

    public MapEditor()
    {
        InitializeComponent();
        RegisterMessages();
        HookCursorEvents();
        _cursorManager = new SquareCursorManager(MapCanvas);
        _cursorManager.HideCursor();
    }


    private float MapScale
    {
        get => _mapScale;
        set => _mapScale = (float)Math.Pow(0.05 * (value + 30), 2);
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
    ///     绑定地图上的光标事件
    /// </summary>
    private void HookCursorEvents()
    {
        // 光标进入
        MapCanvas.PointerEntered += (_, _) => { _cursorManager.ShowCursor(); };
        // 光标离开
        MapCanvas.PointerExited += (_, _) =>
        {
            _cursorManager.HideCursor();
            _isMousePressing = false;
        };
        // 光标移动
        MapCanvas.PointerMoved += (_, args) =>
        {
            if (!_cursorManager.IsVisible) return;

            // 更新笔刷光标
            var color = _viewModel.ClosestMapColor;
            if (color != _cursorManager.GetCursorColor()) _cursorManager.SetCursorColor(color);
            _cursorManager.SetCursorSize(MapScale);

            // 修改像素颜色
            if (_isMousePressing)
            {
                var mapPosition = GetMapPosition(args);
                if (_isMousePressing) TryModifyMapColor(mapPosition.x, mapPosition.y);
            }
        };
        //光标按下
        MapCanvas.PointerPressed += (_, args) =>
        {
            _isMousePressing = true;
            // 修改像素颜色
            var mapPosition = GetMapPosition(args);
            TryModifyMapColor(mapPosition.x, mapPosition.y);
        };
        // 光标松开
        MapCanvas.PointerReleased += (_, _) => { _isMousePressing = false; };


        return;

        (int x, int y) GetMapPosition(PointerRoutedEventArgs args)
        {
            var position = args.GetCurrentPoint(MapCanvas).Position;
            var mapX = Math.Ceiling(position._x / _mapScale);
            var mapY = Math.Ceiling(position._y / _mapScale);
            return ((int)mapX, (int)mapY);
        }
    }

    /// <summary>
    /// 尝试修改地图像素颜色
    /// </summary>
    /// <remarks>当传入地图坐标超出数组，直接返回</remarks>
    private void TryModifyMapColor(int x, int y)
    {
        const int maxIndex = InitMapSize - 1;
        if (x > maxIndex || y > maxIndex) return;
        _viewModel.SetColor(x, y);
        MapCanvas.Invalidate();
        SaveButton.IsEnabled = true;
    }

    private void MapZoom_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        MapCanvas?.Invalidate();
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        btn.IsEnabled = false;
    }
}