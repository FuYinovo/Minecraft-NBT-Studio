using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI.Core;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using NBT_Studio.Component.CustomCursor.Class;
using NBT_Studio.Enum;
using NBT_Studio.Manager;
using NBT_Studio.Message;
using NBT_Studio.ViewModel;

namespace NBT_Studio.View;

public sealed partial class MapEditor : INotifyPropertyChanged
{
    public const int InitMapSize = 128;
    public const int MaxZoomFactor = 80;
    public const int MinZoomFactor = 2;
    public const int MinBrushRadius = 0;
    public const int MaxBrushRadius = 8;
    private readonly MapEditorToolsHandlersManager _eventHandlerManager;

    public readonly SquareCursorManager SquareCursorManager;
    public readonly MapEditorViewModel ViewModel;
    private int _brushRadius;
    private float _mapScale = (float)6.25;
    public bool IsMousePressing;
    public (float x, float y) MapOffset = (0, 0);
    public Vector2 ScaleCenter = new((float)InitMapSize / 2, (float)InitMapSize / 2);

    public MapEditor()
    {
        ViewModel = new MapEditorViewModel(InitMapSize);
        _eventHandlerManager = new MapEditorToolsHandlersManager(this);
        InitializeComponent();
        RegisterMessages();
        HookCursorEvents();
        SquareCursorManager = new SquareCursorManager(MapCanvas);
        SquareCursorManager.HideCursor();
    }


    public float MapScale
    {
        get => _mapScale;
        set
        {
            _mapScale = (float)Math.Pow(0.05 * (value + 30), 2); // f(x) = [(x+30)/20]^2
            MapCanvas?.Invalidate();
        }
    }

    public int BrushRadius
    {
        get => _brushRadius;
        set => SetField(ref _brushRadius, value);
    }


    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<DetailPadSizeChangedMessage>(this,
            (_, v) => UpdateGridSize(v.Value));
        WeakReferenceMessenger.Default.Register<DrawMapMessage>(this,
            (_, _) => MapCanvas.Invalidate());
    }

    ///<summary> 调整内容大小 </summary>
    private void UpdateGridSize(Size size)
    {
        if (size.Width <= 60) return;
        ControlGrid.Width = size.Width - 60;
        ControlGrid.Height = size.Height - 70;
    }

    /// <summary> 使用 Win2D 绘制地图</summary>
    private void DrawMap(CanvasControl sender, CanvasDrawEventArgs args)
    {
        var session = args.DrawingSession;
        session.Transform =
            Matrix3x2.CreateScale(MapScale, MapScale, ScaleCenter) *
            Matrix3x2.CreateTranslation(MapOffset.x, MapOffset.y);
        for (var y = 0; y < InitMapSize; y++)
        for (var x = 0; x < InitMapSize; x++)
            session.FillRectangle(x, y, 1, 1, ViewModel.Pixels[y, x].Color);
    }

    /// <summary> 绑定地图上的光标事件</summary>
    private void HookCursorEvents()
    {
        MapCanvas.PointerEntered += (obj, args) =>
            _eventHandlerManager.GetHandler(GetToolType(args)).HandleEntered(obj, args);
        MapCanvas.PointerExited += (obj, args) =>
            _eventHandlerManager.GetHandler(GetToolType(args)).HandleExited(obj, args);
        MapCanvas.PointerMoved += (obj, args) =>
            _eventHandlerManager.GetHandler(GetToolType(args)).HandleMoved(obj, args);
        MapCanvas.PointerReleased += (obj, args) =>
            _eventHandlerManager.GetHandler(GetToolType(args)).HandleReleased(obj, args);
        MapCanvas.PointerPressed += (obj, args) =>
            _eventHandlerManager.GetHandler(GetToolType(args)).HandlePressed(obj, args);
        MapCanvas.PointerWheelChanged += (obj, args) =>
            _eventHandlerManager.GetHandler(GetToolType(args)).WheelChanged(obj, args);

        return;

        MapEditorTool GetToolType(PointerRoutedEventArgs args)
        {
            // 右键则触发移动工具
            return args.GetCurrentPoint(MapCanvas).Properties.IsRightButtonPressed
                ? MapEditorTool.Move
                : ViewModel.SelectedTool;
        }
    }

    /// <summary> 设置此控件下的光标 </summary>
    public void SetCursor(CoreCursorType cursorType)
    {
        if (cursorType == CoreCursorType.Custom) return;
        ProtectedCursor = InputCursor.CreateFromCoreCursor(new CoreCursor(cursorType, 0));
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        btn.IsEnabled = false;
    }

    public Button GetSaveButton()
    {
        return SaveButton;
    }

    public CanvasControl GetMapCanvas()
    {
        return MapCanvas;
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
}