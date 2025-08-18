using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Control.Dialog;
using NBT_Studio.Enum;
using NBT_Studio.Enum.Settings;
using NBT_Studio.Library.NBT_Parser.Class;
using NBT_Studio.Library.NBT_Parser.Enum;
using NBT_Studio.Message;
using NBT_Studio.Model;
using NBT_Studio.Service;
using WinRT.Interop;
using NBT_Studio.Utils;
using FileInfo = NBT_Studio.Model.FileInfo;

namespace NBT_Studio.ViewModel;

/// <summary>
///     属性、构造方法
/// </summary>
[SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator",
    "MVVMTK0045:Using [ObservableProperty] on fields is not AOT compatible for WinRT")]
public sealed partial class TreeViewPageViewModel : ObservableObject
{
    private FileInfo _fileInfo = new();
    [ObservableProperty] private bool _isApplyEnabled;
    [ObservableProperty] private bool _isBedrockLevelDat;
    [ObservableProperty] private bool _isFilterEnabled;
    [ObservableProperty] private bool _isInfoEnabled;
    [ObservableProperty] private bool _isSaveEnabled;
    [ObservableProperty] private bool _isSearchBoxEnabled;
    private int _nodeFilterIndex;
    [ObservableProperty] private ObservableCollection<NbtNode> _nodes = [];
    private string _searchBoxText = string.Empty;
    [ObservableProperty] private TreeViewNode? _selectedNode;
    [ObservableProperty] private bool _waitingToSelectMaskVisibility = true;

    public TreeViewPageViewModel()
    {
        RegisterMessages();
    }

    public int NodeFilterIndex
    {
        get => _nodeFilterIndex;
        set
        {
            SetField(ref _nodeFilterIndex, value);
            ApplyFilter();
        }
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

    /// <summary>
    ///     注册消息队列
    /// </summary>
    private void RegisterMessages()
    {
        // 「选中节点改动」消息
        WeakReferenceMessenger.Default.Register<SelectedNodeChangedMessage>(this,
            (_, v) =>
            {
                SelectedNode = v.Value;
                WaitingToSelectMaskVisibility = false;
            });
        // 「排序方式改动」消息
        WeakReferenceMessenger.Default.Register<SettingsChangedMessage<Sort>>(this,
            (_, v) => ApplySort(v.Value));
        // 「节点被修改」消息
        WeakReferenceMessenger.Default.Register<NodeModifiedMessage>(this,
            (_, _) =>
            {
                if (_fileInfo.FilePath != string.Empty) IsApplyEnabled = true;
            });
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(propertyName);
    }
}

/// <summary>
///     文件操作、
/// </summary>
public sealed partial class TreeViewPageViewModel
{
    /// <summary>加载 NBT 文件</summary>
    [RelayCommand]
    private async Task LoadFile()
    {
        // 确认是否丢弃修改
        if (IsApplyEnabled)
        {
            var choice = await VerifyAbandonModifies();
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
        var hWnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(openPicker, hWnd);

        // 选择文件
        var file = await openPicker.PickSingleFileAsync();
        if (file == null) return;
        _fileInfo.FilePath = file.Path;

        // 读取文件
        var fileStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read);
        var compressInfo = CompressFileHelper.IsCompressedFile(fileStream);
        byte[] bytes;
        if (compressInfo.isCompressed)
        {
            bytes = CompressFileHelper.DecompressFile(fileStream, (FileCompress)compressInfo.compressType!);
            _fileInfo.IsCompressed = true;
            _fileInfo.CompressType = (FileCompress)compressInfo.compressType!;
        }
        else
        {
            var binaryReader = new BinaryReader(fileStream);
            bytes = binaryReader.ReadBytes((int)fileStream.Length);
            binaryReader.Close();
        }

        fileStream.Close();

        // 分别尝试以 Java 版、基岩版加载
        var javaResult = LoadJavaFile(bytes);
        if (javaResult.isSuccessed)
        {
            _fileInfo.MinecraftEdition = MinecraftEdition.Java;
            _fileInfo.Endianness = Endianness.Big;
            LoadRootTag(javaResult.tag);
            return;
        }

        var bedrockResult = LoadBedrockFile(bytes);
        if (bedrockResult.isSuccessed)
        {
            _fileInfo.MinecraftEdition = MinecraftEdition.Bedrock;
            _fileInfo.Endianness = Endianness.Little;
            IsBedrockLevelDat = bedrockResult.isLevelDat;
            LoadRootTag(bedrockResult.tag);
            return;
        }

        await DialogService.ShowDialog("加载失败", "确认", description: "请检查 NBT 文件是否损坏");
        return;

        # region Methods

        void LoadRootTag(NbtTag rootTag)
        {
            Nodes.Clear();
            Nodes.Add(new NbtNode(rootTag, null, true));

            IsSaveEnabled = true;
            IsApplyEnabled = false;
            IsInfoEnabled = true;
            IsFilterEnabled = true;
            IsSearchBoxEnabled = true;
        }

        // 对于基岩版文件，分别尝试从 0 、8 开始解析，若均失败，则弹窗失败
        static (bool isSuccessed, NbtTag tag, bool isLevelDat ) LoadBedrockFile(byte[] bytes, int begin = 0,
            int tired = 1)
        {
            try
            {
                return (true, new NbtParser().Parse(bytes, false, begin), begin != 0);
            }
            catch (Exception)
            {
                if (tired <= 2) return LoadBedrockFile(bytes, begin == 0 ? 8 : 0, tired + 1);
                return (false, null, false)!;
            }
        }

        static (bool isSuccessed, NbtTag tag) LoadJavaFile(byte[] bytes)
        {
            try
            {
                return (true, new NbtParser().Parse(bytes, true));
            }
            catch (Exception)
            {
                return (false, null)!;
            }
        }

        #endregion
    }

    /// <summary>将 NBT 文件另存为</summary>
    [RelayCommand]
    private async Task SaveFile()
    {
        // 获取字节数组
        var bytes = Nodes.First().Tag.GetBytes();

        // 初始化 Picker
        var savePicker = new FileSavePicker();
        savePicker.FileTypeChoices.Add("NBT Files", new List<string> { ".nbt", ".dat" });
        savePicker.SuggestedFileName =
            string.IsNullOrWhiteSpace(Nodes.First().DisplayName) ? "unnamed_nbt_file" : Nodes.First().DisplayName;
        var hWnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(savePicker, hWnd);

        // 选择保存位置
        var path = await savePicker.PickSaveFileAsync();
        if (path == null) return;
        await WriteFile(bytes, path.Path);
        _fileInfo.FilePath = path.Path;
        IsApplyEnabled = false;
    }

    /// <summary>保存 NBT 文件</summary>
    [RelayCommand]
    private async Task ApplyFile()
    {
        var bytes = Nodes.First().Tag.GetBytes();
        await WriteFile(bytes, _fileInfo.FilePath);

        IsApplyEnabled = false;
    }

    /// <summary>创建 NBT 文件</summary>
    [RelayCommand]
    private async Task CreateFile(MinecraftEdition? param)
    {
        // 确认是否丢弃修改
        if (IsApplyEnabled)
            switch (await VerifyAbandonModifies())
            {
                case ContentDialogResult.None: // 结束方法
                    return;
                case ContentDialogResult.Primary:
                    await ApplyFile(); // 应用并执行方法
                    break;
                case ContentDialogResult.Secondary:
                    break; // 执行方法
            }

        _fileInfo.MinecraftEdition = param ?? throw new Exception("尝试新建未知设置游戏版本!");
        var builder = new NbtTagBuilder(_fileInfo.Endianness == Endianness.Big);
        Nodes.Clear();
        Nodes.Add(new NbtNode(builder.Dictionary("root", []), null, true));
        _fileInfo.FilePath = string.Empty;
        IsApplyEnabled = false;
        IsSaveEnabled = true;
        IsInfoEnabled = true;
        IsFilterEnabled = true;
        IsSearchBoxEnabled = true;
    }

    /// <summary> 展示 NBT 文件信息 </summary>
    [RelayCommand]
    private async Task ShowFileInfo()
    {
        _fileInfo.FileLength = Nodes.First().Tag.GetBytes().Length;
        var content = new FileInfoDialog(_fileInfo);
        await DialogService.ShowDialog("文件信息", "确认", content: content);
    }

    /// <summary>写入 NBT 文件</summary>
    private async Task WriteFile(byte[] bytes, string path)
    {
        // 写入文件
        switch (_fileInfo.MinecraftEdition)
        {
            case MinecraftEdition.Java:
                await WriteBytes(bytes);
                break;
            case MinecraftEdition.Bedrock:
                if (!IsBedrockLevelDat)
                {
                    await WriteBytes(bytes);
                    break;
                }

                // 基岩版的 level.dat 文件前 8 字节是两个整数：10 和 数据长度
                var extension = new byte[8];
                BinaryPrimitives.WriteInt32LittleEndian(extension, 10);
                BinaryPrimitives.WriteInt32LittleEndian(extension, bytes.Length);
                await WriteBytes(extension.Concat(bytes).ToArray());
                break;
            default:
                throw new Exception("保存文件时无法确定游戏版本");
        }

        return;

        // 根据 _fileInfo 决定是否压缩后保存
        async Task WriteBytes(byte[] data)
        {
            await using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);

            var compressType = _fileInfo.CompressType;
            await using Stream outStream = _fileInfo.IsCompressed switch
            {
                true when compressType == FileCompress.Gzip => new GZipStream(fileStream, CompressionMode.Compress),
                true when compressType == FileCompress.Zlib => new ZLibStream(fileStream, CompressionMode.Compress),
                false => fileStream,
                _ => throw new NotSupportedException($"不支持对将文件压缩为[{compressType}]类型!")
            };
            await outStream.WriteAsync(data);
        }
    }

    /// <summary>确认是否丢弃 NBT 文件未保存的修改</summary>
    private async Task<ContentDialogResult> VerifyAbandonModifies()
    {
        var fileName = _fileInfo.FilePath == string.Empty
            ? Nodes.First().DisplayName == string.Empty ? "未命名" : Nodes.First().DisplayName
            : Path.GetFileNameWithoutExtension(_fileInfo.FilePath);
        return await DialogService.ShowDialog("保存修改？", "保存", "丢弃", "取消", description: $"「{fileName}」未保存修改");
    }
}

/// <summary>
///     界面更新、
/// </summary>
public sealed partial class TreeViewPageViewModel
{
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
                    if (NbtTagEnumExtensions.IsCollection(child.TagEnum)) continue;
                    child.Visibility = child.TagEnum != filterEnum ? Visibility.Collapsed : Visibility.Visible;
                }

                break;
        }

        // 二、应用节点搜索
        foreach (var child in childrenAll.Where(child =>
                     !string.IsNullOrWhiteSpace(SearchBoxText) && !child.DisplayName.Contains(SearchBoxText) &&
                     !NbtTagEnumExtensions.IsCollection(child.TagEnum)))
            child.Visibility = Visibility.Collapsed;

        // 三、 隐藏空的列表、字典
        foreach (var child in childrenAll)
            if (NbtTagEnumExtensions.IsCollection(child.TagEnum) && child.GetVisibleChildrenCount() <= 0)
                child.Visibility = Visibility.Collapsed;

        // 四、显示根节点
        Nodes.First().Visibility = Visibility.Visible;

        // 四、更新子项数量
        foreach (var child in childrenAll) child.UpdateChildrenCount();
    }

    /// <summary> 应用节点排序 </summary>
    private void ApplySort(Sort type)
    {
        // TODO)) 节点排序
    }
}

/// <summary>
///     数据操作
/// </summary>
public sealed partial class TreeViewPageViewModel
{
    /// <summary>
    ///     插入节点
    /// </summary>
    /// <remarks>调用 <see cref="NbtNode" /> 的 AppendChild 方法（含界面交互）</remarks>
    [RelayCommand]
    private async Task AppendNode(NbtTagEnum tagEnum)
    {
        if (SelectedNode == null)
        {
            await DialogService.ShowDialog("添加失败", "确认", description: "选择一个父节点或其子项");
            return;
        }

        var selectedNbtNode = (NbtNode)SelectedNode.Content;
        await selectedNbtNode.AppendChild(tagEnum);

        if (_fileInfo.FilePath != string.Empty) IsApplyEnabled = true;
        IsSaveEnabled = true;
    }
}