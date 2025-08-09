using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Control.Content;
using NBT_Studio.Enum;
using NBT_Studio.Library.NBT_Parser.Class;
using NBT_Studio.Library.NBT_Parser.Enum;
using NBT_Studio.Library.NBT_Parser.Utils;
using NBT_Studio.Model;
using NBT_Studio.Service;
using WinRT.Interop;

namespace NBT_Studio.ViewModel;

/// <summary>
/// 文件操作、节点筛选、
/// </summary>
public sealed partial class TreeViewPageViewModel : INotifyPropertyChanged
{
    public TreeViewPageViewModel()
    {
        LoadFileCommand = new AsyncRelayCommand<string>(LoadFile);
        SaveFileCommand = new AsyncRelayCommand(SaveFile);
        ApplyFileCommand = new AsyncRelayCommand(ApplyFile);
        ShowFileInfoCommand = new AsyncRelayCommand(ShowFileInfo);
        CreateFileCommand = new AsyncRelayCommand<string>(CreateFile);
    }

    /// <summary>加载 NBT 文件</summary>
    private async Task LoadFile(string? param)
    {
        // 确认是否丢弃修改
        if (IsApplyEnabled)
        {
            var choice = await VerifyAbandonChanges();
            switch (choice)
            {
                case ContentDialogResult.None: // 结束方法
                    return;
                case ContentDialogResult.Primary:
                    await ApplyFile(); // 应用并执行方法
                    break;
                case ContentDialogResult.Secondary:
                    break; // 执行方法
                default: throw new Exception($"未知的对话框选择[{choice}]");
            }
        }

        // 初始化 Picker
        var openPicker = new FileOpenPicker
        {
            ViewMode = PickerViewMode.Thumbnail
        };
        openPicker.FileTypeFilter.Add(".nbt");
        openPicker.FileTypeFilter.Add(".dat");
        var hWnd = WindowNative.GetWindowHandle(App.Window);
        InitializeWithWindow.Initialize(openPicker, hWnd);

        // 选择文件
        var file = await openPicker.PickSingleFileAsync();
        if (file == null) return;

        // 读取文件
        _filePath = file.Path;
        var bytes = Tools.ReadBytes(file.Path);
        _gameEdition = param?.ToLower() switch
        {
            "java" => GameEditionEnum.Java,
            "bedrock" => GameEditionEnum.Bedrock,
            _ => throw new Exception("新建文件按钮在XAML中版本参数错误!")
        };
        // Java | Bedrock 分类处理
        NbtTag rootTag;
        switch (_gameEdition)
        {
            case GameEditionEnum.Java:
                var tag = await TryLoadJavaFile(bytes);
                if (tag == null) return;
                rootTag = tag;
                break;
            case GameEditionEnum.Bedrock:
                var result = await TryLoadBedrockFile(bytes);
                if (result.tag == null) return;
                _isBedrockLevelDat = result.isLevelDat;
                rootTag = result.tag;
                break;
            default:
                throw new Exception($"未知游戏版本[{_gameEdition}]");
        }

        Nodes.Clear();
        Nodes.Add(new NbtNode(rootTag, true, NodeChangeAction));

        IsSaveEnabled = true;
        IsApplyEnabled = false;
        IsInfoEnabled = true;
        IsFilterEnabled = true;
        IsSearchBoxEnabled = true;

        return;

        // 对于基岩版文件，分别尝试从 0 、8 开始解析，若均失败，则弹窗失败
        static async Task<(NbtTag? tag, bool isLevelDat)> TryLoadBedrockFile(byte[] bytes, int begin = 0, int tired = 1)
        {
            try
            {
                return (new NbtParser().Parse(bytes, false, begin), begin != 0);
            }
            catch (Exception)
            {
                if (tired <= 2) return await TryLoadBedrockFile(bytes, begin == 0 ? 8 : 0, tired + 1);
                await DialogService.ShowDialog("加载失败", "确认", description: "请确保选择了正确的游戏版本");
                return (null, false);
            }
        }

        static async Task<NbtTag?> TryLoadJavaFile(byte[] bytes)
        {
            try
            {
                return new NbtParser().Parse(bytes, true);
            }
            catch (Exception)
            {
                await DialogService.ShowDialog("加载失败", "确认", description: "请确保选择了正确的游戏版本");
                return null;
            }
        }
    }

    /// <summary>将 NBT 文件另存为</summary>
    private async Task SaveFile()
    {
        // 获取字节数组
        var bytes = Nodes.First().Tag.GetBytes();

        // 初始化 Picker
        var savePicker = new FileSavePicker();
        savePicker.FileTypeChoices.Add("NBT Files", new List<string> { ".nbt", ".dat" });
        savePicker.SuggestedFileName =
            string.IsNullOrWhiteSpace(_nodes.First().Name) ? "unnamed_nbt_file" : _nodes.First().Name;
        var hWnd = WindowNative.GetWindowHandle(App.Window);
        InitializeWithWindow.Initialize(savePicker, hWnd);

        // 选择保存位置
        var path = await savePicker.PickSaveFileAsync();
        if (path == null) return;
        await WriteFile(bytes, path.Path);

        _filePath = path.Path;
        IsApplyEnabled = false;
    }

    /// <summary>保存 NBT 文件</summary>
    private async Task ApplyFile()
    {
        var bytes = Nodes.First().Tag.GetBytes();
        await WriteFile(bytes, _filePath);

        IsApplyEnabled = false;
    }

    /// <summary>创建 NBT 文件</summary>
    private async Task CreateFile(string? param)
    {
        // 确认是否丢弃修改
        if (IsApplyEnabled)
            switch (await VerifyAbandonChanges())
            {
                case ContentDialogResult.None: // 结束方法
                    return;
                case ContentDialogResult.Primary:
                    await ApplyFile(); // 应用并执行方法
                    break;
                case ContentDialogResult.Secondary:
                    break; // 执行方法
            }

        // 新建文件
        _gameEdition = param?.ToLower() switch
        {
            "java" => GameEditionEnum.Java,
            "bedrock" => GameEditionEnum.Bedrock,
            _ => throw new Exception("新建文件按钮在XAML中版本参数错误!")
        };
        var builder = new NbtTagBuilder(_gameEdition == GameEditionEnum.Java);
        if (Nodes.Count > 0) Nodes.Clear();
        Nodes.Clear();
        Nodes.Add(new NbtNode(builder.Dictionary("root", []), true, NodeChangeAction));

        _filePath = string.Empty;
        IsApplyEnabled = false;
        IsSaveEnabled = true;
        IsInfoEnabled = true;
        IsFilterEnabled = true;
        IsSearchBoxEnabled = true;
    }

    /// <summary> 应用节点筛选</summary>
    private void ApplyFilter()
    {
        // 一、应用节点筛选
        var filterEnum = _nodeFilterIndex switch
        {
            1 => NbtTagEnum.Byte,
            2 => NbtTagEnum.Short,
            3 => NbtTagEnum.Int,
            4 => NbtTagEnum.Long,
            5 => NbtTagEnum.Float,
            6 => NbtTagEnum.Double,
            7 => NbtTagEnum.String,
            _ => NbtTagEnum.Unknown
        };
        var childrenAll = Nodes.First().GetChildrenAll();
        switch (filterEnum)
        {
            // 情景一：选择了「全部」筛选标签
            case NbtTagEnum.Unknown:
                // 全部显示
                foreach (var child in childrenAll)
                    child.Visibility = Visibility.Visible;
                break;
            // 情景二：选择了其他筛选标签
            default:
                // 隐藏所有非目标节点（列表、字典除外）
                foreach (var child in childrenAll)
                {
                    if (child.TagEnum is NbtTagEnum.Dictionary or NbtTagEnum.List) continue;
                    child.Visibility = child.TagEnum != filterEnum ? Visibility.Collapsed : Visibility.Visible;
                }

                break;
        }

        // 二、应用节点搜索
        foreach (var child in childrenAll.Where(child =>
                     !string.IsNullOrWhiteSpace(SearchBoxText) && !child.Name.Contains(SearchBoxText) &&
                     child.TagEnum is not (NbtTagEnum.Dictionary or NbtTagEnum.List)))
            child.Visibility = Visibility.Collapsed;


        // 三、 隐藏空的列表、字典
        foreach (var child in childrenAll)
            if (child.TagEnum is NbtTagEnum.Dictionary or NbtTagEnum.List && child.GetVisibleChildrenCount() <= 0)
                child.Visibility = Visibility.Collapsed;


        // 四、更新子项数量（方法自动过滤非列表、字典节点）
        foreach (var child in childrenAll) child.UpdateChildrenCount();
    }

    /// <summary> 展示 NBT 文件信息 </summary>
    private async Task ShowFileInfo()
    {
        var content = new NbtFileInfoContent(_filePath, _nodes.First().Tag.GetBytes().Length, _gameEdition,
            _nodes.First().Tag.IsBigEndian);
        await DialogService.ShowDialog("文件信息", "确认", content: content);
    }

    /// <summary>写入 NBT 文件</summary>
    private async Task WriteFile(byte[] bytes, string path)
    {
        var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
        switch (_gameEdition)
        {
            case GameEditionEnum.Java:
                await fileStream.WriteAsync(bytes);
                break;
            case GameEditionEnum.Bedrock:
                if (!_isBedrockLevelDat)
                {
                    await fileStream.WriteAsync(bytes);
                    break;
                }

                // 基岩版的 level.dat 文件前 8 字节是两个整数：10 和 数据长度
                var extension = new byte[8];
                BinaryPrimitives.WriteInt32LittleEndian(extension, 10);
                BinaryPrimitives.WriteInt32LittleEndian(extension, bytes.Length);
                await fileStream.WriteAsync(extension.Concat(bytes).ToArray());
                break;
            default:
                throw new Exception("保存文件时无法确定游戏版本");
        }

        fileStream.Close();
    }

    /// <summary>确认是否丢弃 NBT 文件未保存的修改</summary>
    private async Task<ContentDialogResult> VerifyAbandonChanges()
    {
        var fileName = _filePath == string.Empty
            ? _nodes.First().Name == string.Empty ? "未命名" : _nodes.First().Name
            : Path.GetFileNameWithoutExtension(_filePath);
        return await DialogService.ShowDialog("是否保存修改？", "保存", "丢弃", "取消", description: $"「{fileName}」未保存修改");
    }

    /// <summary> 处理节点修改操作相关 UI 更新 </summary>
    /// <remarks> 委托方法</remarks>
    private void NodeChangeAction(NbtNode node, NodeChangeType type)
    {
        switch (type)
        {
            case NodeChangeType.Rename:
                break;
            case NodeChangeType.Remove:
                node.Visibility = Visibility.Collapsed;
                foreach (var child in node.Children) child.Visibility = Visibility.Collapsed;
                node.UpdateChildrenCount();
                Nodes.Remove(node);
                break;
            case NodeChangeType.Revalue:
                break;
            default:
                throw new Exception("无法确定节点修改操作类型");
        }

        IsApplyEnabled = true;
    }

    #region Properties

    public event PropertyChangedEventHandler? PropertyChanged;
    private ObservableCollection<NbtNode> _nodes = [];
    private bool _isSaveEnabled;
    private bool _isApplyEnabled;
    private bool _isInfoEnabled;
    private bool _isFilterEnabled;
    private bool _isSearchBoxEnabled;
    private int _nodeFilterIndex;
    private string _searchBoxText = string.Empty;

    public int NodeFilterIndex
    {
        get => _nodeFilterIndex;
        set
        {
            SetField(ref _nodeFilterIndex, value);
            ApplyFilter();
        }
    }

    private string _filePath = string.Empty;
    private bool _isBedrockLevelDat;
    private GameEditionEnum _gameEdition;
    public IAsyncRelayCommand<string> LoadFileCommand { get; }
    public IAsyncRelayCommand SaveFileCommand { get; }
    public IAsyncRelayCommand ApplyFileCommand { get; }
    public IAsyncRelayCommand ShowFileInfoCommand { get; }
    public IAsyncRelayCommand<string> CreateFileCommand { get; }

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

    public bool IsFilterEnabled
    {
        get => _isFilterEnabled;
        set => SetField(ref _isFilterEnabled, value);
    }

    public bool IsApplyEnabled
    {
        get => _isApplyEnabled;
        set => SetField(ref _isApplyEnabled, value);
    }

    public bool IsInfoEnabled
    {
        get => _isInfoEnabled;
        set => SetField(ref _isInfoEnabled, value);
    }

    public bool IsSearchBoxEnabled
    {
        get => _isSearchBoxEnabled;
        set => SetField(ref _isSearchBoxEnabled, value);
    }

    public string SearchBoxText
    {
        get => _searchBoxText;
        set
        {
            SetField(ref _searchBoxText, value);
            ApplyFilter();
        }
    }

    #endregion


    #region INotifyPropertyChanged

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

/// <summary>
/// 数据操作
/// </summary>
public sealed partial class TreeViewPageViewModel
{
}