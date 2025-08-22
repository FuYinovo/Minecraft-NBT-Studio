using System;
using System.Numerics;
using Windows.Foundation;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;
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
        MapCanvas.PointerEntered += (_, _) => { _cursorManager.ShowCursor(); }; // 光标进入
        MapCanvas.PointerExited += (_, _) => { _cursorManager.HideCursor(); }; // 光标离开
        MapCanvas.PointerMoved += (_, args) => // 光标移动
        {
            if (!_cursorManager.IsVisible) return;
            var position = args.GetCurrentPoint(MapCanvas).Position;
            // 更新笔刷光标
            var color = _viewModel.ClosestMapColor;
            if (color != _cursorManager.GetCursorColor())
            {
                _cursorManager.SetCursorColor(color);
                _cursorManager.SetCursorSize(MapScale);
            }

            // 修改像素颜色
            if (_isMousePressing)
            {
                var mapPosition = GetMapPosition(args);
                if (_isMousePressing) TryModifyMapColor(mapPosition.x, mapPosition.y);
            }
        };
        MapCanvas.PointerPressed += (_, args) => //光标按下
        {
            _isMousePressing = true;
            // 修改像素颜色
            var mapPosition = GetMapPosition(args);
            TryModifyMapColor(mapPosition.x, mapPosition.y);
        };
        MapCanvas.PointerReleased += (_, _) => // 光标松开
        {
            _isMousePressing = false;
        };


        return;

        (float x, float y) GetAdsorbPosition(float x, float y)
        {
            return ((float, float))(Math.Floor(x / MapScale) * MapScale, Math.Floor(y / MapScale) * MapScale);
        }

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
    }

    private void MapZoom_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        MapCanvas?.Invalidate();
    }
}