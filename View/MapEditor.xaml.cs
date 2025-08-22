using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Core;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Component.CustomCursor.Class;
using NBT_Studio.Manager;
using NBT_Studio.Message;
using NBT_Studio.ViewModel;

namespace NBT_Studio.View;

public sealed partial class MapEditor
{
    private const int InitMapSize = 128;
    public readonly SquareCursorManager SquareCursorManager;
    public readonly MapEditorViewModel ViewModel = new(InitMapSize);
    private float _mapScale = 1;
    public bool IsMousePressing;
    private readonly MapEditorToolsHandlersManager _eventHandlerManager;

    public MapEditor()
    {
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
            _mapScale = (float)Math.Pow(0.05 * (value + 30), 2);
            MapCanvas?.Invalidate();
        }
    }

    public CanvasControl MapCanvasControl => MapCanvas;
    public Button SaveButtonControl => SaveButton;

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
        session.Transform = Matrix3x2.CreateScale(MapScale, MapScale);
        for (var y = 0; y < InitMapSize; y++)
        for (var x = 0; x < InitMapSize; x++)
            session.FillRectangle(x, y, 1, 1, ViewModel.Pixels[y, x].Color);
    }

    /// <summary> 绑定地图上的光标事件</summary>
    private void HookCursorEvents()
    {
        MapCanvas.PointerEntered += (obj, args) =>
            _eventHandlerManager.GetHandler(ViewModel.SelectedTool).HandleEntered(obj, args);
        MapCanvas.PointerExited += (obj, args) =>
            _eventHandlerManager.GetHandler(ViewModel.SelectedTool).HandleExited(obj, args);
        MapCanvas.PointerMoved += (obj, args) =>
            _eventHandlerManager.GetHandler(ViewModel.SelectedTool).HandleMoved(obj, args);
        MapCanvas.PointerPressed += (obj, args) =>
            _eventHandlerManager.GetHandler(ViewModel.SelectedTool).HandlePressed(obj, args);
        MapCanvas.PointerReleased += (obj, args) =>
            _eventHandlerManager.GetHandler(ViewModel.SelectedTool).HandleReleased(obj, args);
    }

    /// <summary>  设置此控件下的光标 </summary>
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
}