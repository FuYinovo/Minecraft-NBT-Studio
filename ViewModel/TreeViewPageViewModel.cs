using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using NBT_Parser.Class;
using NBT_Studio.Model;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using NBT_Parser.Utils;

namespace NBT_Studio.ViewModel;

public class TreeViewPageViewModel : INotifyPropertyChanged
{
    #region Properties

    public event PropertyChangedEventHandler? PropertyChanged;
    private ObservableCollection<NbtNode> _nodes = [];
    private bool _isSaveEnabled;
    private bool _isApplyEnabled;
    private string _filePath = string.Empty;
    public IAsyncRelayCommand LoadFileCommand { get; set; }
    public IAsyncRelayCommand SaveFileCommand { get; set; }
    public IAsyncRelayCommand ApplyFileCommand { get; set; }
    public IAsyncRelayCommand CreateFileCommand { get; set; }

    public ObservableCollection<NbtNode> Nodes
    {
        get => _nodes;
        set => SetField(ref _nodes, value);
    }

    public bool IsSaveEnabled
    {
        get => _isSaveEnabled;
        set => SetField(ref _isSaveEnabled, value);
    }

    public bool IsApplyEnabled
    {
        get => _isApplyEnabled;
        set => SetField(ref _isApplyEnabled, value);
    }

    #endregion

    public TreeViewPageViewModel()
    {
        LoadFileCommand = new AsyncRelayCommand(LoadFile);
        SaveFileCommand = new AsyncRelayCommand(SaveFile);
        ApplyFileCommand = new AsyncRelayCommand(ApplyFile);
        CreateFileCommand = new AsyncRelayCommand(CreateFile);
    }


    private async Task LoadFile()
    {
        // 确认是否丢弃修改
        if (IsApplyEnabled)
        {
            var decision = await Service.DialogService.ShowDialog("是否保存修改？", "保存", "丢弃");
            switch (decision)
            {
                case ContentDialogResult.None: // 结束方法
                    return;
                case ContentDialogResult.Primary:
                    await ApplyFile(); // 应用并执行方法
                    break;
                case ContentDialogResult.Secondary:
                    break; // 执行方法
            }
        }

        // 初始化 Picker
        var openPicker = new FileOpenPicker
        {
            ViewMode = PickerViewMode.Thumbnail
        };
        openPicker.FileTypeFilter.Add(".nbt");
        openPicker.FileTypeFilter.Add(".dat");
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.Window);
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        // 选择文件
        var file = await openPicker.PickSingleFileAsync();
        if (file == null) return;

        // 读取文件
        _filePath = file.Path;
        var bytes = Tools.ReadBytes(file.Path);
        var fileInfo = Tools.GetNbtBytesInfo(bytes);
        var rootTag = new NbtParser().Parse(bytes, fileInfo.isBigEndian, fileInfo.begin);
        if (Nodes.Count > 0) Nodes.Clear();
        Nodes.Add(new NbtNode(rootTag));

        IsSaveEnabled = true;
        IsApplyEnabled = false;
    }

    private async Task SaveFile()
    {
        // 获取字节数组
        var bytes = Nodes.First().Tag.GetBytes();

        // 初始化 Picker
        var savePicker = new FileSavePicker();
        savePicker.FileTypeChoices.Add("NBT Files", new List<string> { ".nbt", ".dat" });
        savePicker.SuggestedFileName =
            string.IsNullOrWhiteSpace(_nodes.First().Name) ? "unnamed_nbt_file" : _nodes.First().Name;
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.Window);
        WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

        // 选择保存位置
        var path = await savePicker.PickSaveFileAsync();
        if (path != null) await WriteFile(bytes, path.Path);

        IsApplyEnabled = false;
    }

    private async Task ApplyFile()
    {
        var bytes = Nodes.First().Tag.GetBytes();
        await WriteFile(bytes, _filePath);

        IsApplyEnabled = false;
    }

    private async Task CreateFile()
    {
        // 确认是否丢弃修改
        if (IsApplyEnabled)
        {
            var decision = await Service.DialogService.ShowDialog("是否保存修改？", "保存", "丢弃");
            switch (decision)
            {
                case ContentDialogResult.None: // 结束方法
                    return;
                case ContentDialogResult.Primary:
                    await ApplyFile(); // 应用并执行方法
                    break;
                case ContentDialogResult.Secondary:
                    break; // 执行方法
            }
        }

        // 新建文件
        var builder = new NbtTagBuilder(true);
        if (Nodes.Count > 0) Nodes.Clear();
        Nodes.Clear();
        Nodes.Add(new NbtNode(builder.Dictionary("root", [])));

        IsApplyEnabled = false;
    }

    private static async Task WriteFile(byte[] bytes, string path)
    {
        var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
        await fileStream.WriteAsync(bytes);
        fileStream.Close();
    }


    #region INotifyPropertyChanged

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
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