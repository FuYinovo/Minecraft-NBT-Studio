using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Enum;
using NBT_Studio.Enum.Settings;
using NBT_Studio.Library.NBT_Parser.Class;
using NBT_Studio.Library.NBT_Parser.Enum;
using NBT_Studio.Library.NBT_Parser.Utils;
using NBT_Studio.Message;
using NBT_Studio.Model;
using NBT_Studio.Service;
using WinRT.Interop;
using AddNodeContent = NBT_Studio.Control.Dialog.AddNodeContent;
using FileInfoContent = NBT_Studio.Control.Dialog.FileInfoContent;

namespace NBT_Studio.ViewModel;

/// <summary>
///     属性、构造方法
/// </summary>
[SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator",
    "MVVMTK0045:Using [ObservableProperty] on fields is not AOT compatible for WinRT")]
public sealed partial class TreeViewPageViewModel : ObservableObject
{
    [ObservableProperty] private string _filePath = string.Empty;
    [ObservableProperty] private GameEdition _gameEdition;
    [ObservableProperty] private bool _isApplyEnabled;
    [ObservableProperty] private bool _isBedrockLevelDat;
    [ObservableProperty] private bool _isFilterEnabled;
    [ObservableProperty] private bool _isInfoEnabled;
    [ObservableProperty] private bool _isSaveEnabled;
    [ObservableProperty] private bool _isSearchBoxEnabled;
    [ObservableProperty] private ObservableCollection<NbtNode> _nodes = [];
    [ObservableProperty] private TreeViewNode? _selectedNode;
    [ObservableProperty] private bool _waitingToSelectMaskVisibility = true;
    private string _searchBoxText = string.Empty;
    private int _nodeFilterIndex;

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

    public TreeViewPageViewModel()
    {
        RegisterMessages();
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
        WeakReferenceMessenger.Default.Register<SettingsValueChangedMessage<Sort>>(this,
            (_, v) => ApplySort(v.Value));
        // 「节点被修改」消息
        WeakReferenceMessenger.Default.Register<NodeChangedMessage>(this,
            (_, v) => NodeChangeAction(v.Value.node, v.Value.type));
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
        var hWnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(openPicker, hWnd);

        // 选择文件
        var file = await openPicker.PickSingleFileAsync();
        if (file == null) return;
        FilePath = file.Path;
        var bytes = Tools.ReadBytes(file.Path);

        // 分别尝试以 Java 版、基岩版加载
        var javaResult = LoadJavaFile(bytes);
        if (javaResult.isSuccessed)
        {
            GameEdition = GameEdition.Java;
            LoadRootTag(javaResult.tag);
            return;
        }

        var bedrockResult = LoadBedrockFile(bytes);
        if (bedrockResult.isSuccessed)
        {
            GameEdition = GameEdition.Bedrock;
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
            Nodes.Add(new NbtNode(rootTag, true));

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
            string.IsNullOrWhiteSpace(Nodes.First().Name) ? "unnamed_nbt_file" : Nodes.First().Name;
        var hWnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(savePicker, hWnd);

        // 选择保存位置
        var path = await savePicker.PickSaveFileAsync();
        if (path == null) return;
        await WriteFile(bytes, path.Path);
        FilePath = path.Path;
        IsApplyEnabled = false;
    }

    /// <summary>保存 NBT 文件</summary>
    [RelayCommand]
    private async Task ApplyFile()
    {
        var bytes = Nodes.First().Tag.GetBytes();
        await WriteFile(bytes, FilePath);

        IsApplyEnabled = false;
    }

    /// <summary>创建 NBT 文件</summary>
    [RelayCommand]
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

        GameEdition = param?.ToLower() switch
        {
            "java" => GameEdition.Java,
            "bedrock" => GameEdition.Bedrock,
            _ => throw new Exception("新建文件按钮在XAML中版本参数错误!")
        };
        var builder = new NbtTagBuilder(GameEdition == GameEdition.Java);
        if (Nodes.Count > 0) Nodes.Clear();
        Nodes.Clear();
        Nodes.Add(new NbtNode(builder.Dictionary("root", []), true));
        FilePath = string.Empty;
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
        var content = new FileInfoContent(FilePath, Nodes.First().Tag.GetBytes().Length, GameEdition,
            Nodes.First().Tag.IsBigEndian);
        await DialogService.ShowDialog("文件信息", "确认", content: content);
    }

    /// <summary>写入 NBT 文件</summary>
    private async Task WriteFile(byte[] bytes, string path)
    {
        var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
        switch (GameEdition)
        {
            case GameEdition.Java:
                await fileStream.WriteAsync(bytes);
                break;
            case GameEdition.Bedrock:
                if (!IsBedrockLevelDat)
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
        var fileName = FilePath == string.Empty
            ? Nodes.First().Name == string.Empty ? "未命名" : Nodes.First().Name
            : Path.GetFileNameWithoutExtension(FilePath);
        return await DialogService.ShowDialog("是否保存修改？", "保存", "丢弃", "取消", description: $"「{fileName}」未保存修改");
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
                     !string.IsNullOrWhiteSpace(SearchBoxText) && !child.Name.Contains(SearchBoxText) &&
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

    /// <summary> 处理节点修改操作相关 UI 更新 </summary>
    /// <remarks> 委托方法</remarks>
    private void NodeChangeAction(NbtNode node, NodeChangeType type)
    {
        switch (type)
        {
            case NodeChangeType.Rename:
                break;
            case NodeChangeType.Remove:
                Nodes.Remove(node);
                // 隐藏自身及其子项
                node.Visibility = Visibility.Collapsed;
                foreach (var child in node.Children) child.Visibility = Visibility.Collapsed;
                // 更新其父节点的子项数量显示
                foreach (var child in Nodes.First().GetChildrenAll()) child.UpdateChildrenCount();
                break;
            case NodeChangeType.SetValue:
                break;
            default:
                throw new Exception("无法确定节点修改操作类型");
        }

        IsApplyEnabled = true;
    }
}

/// <summary>
///     数据操作
/// </summary>
public sealed partial class TreeViewPageViewModel
{
    /// <summary>
    ///     添加节点
    /// </summary>
    [RelayCommand]
    private async Task AddNode(NbtTagEnum tagEnum)
    {
        if (SelectedNode == null)
        {
            await DialogService.ShowDialog("添加失败", "确认", description: "选择一个父节点或其子项");
            return;
        }

        // 确认父类目标（若「选中」是列表或字典，则为自身添加子项，否则为父节点添加子项）
        var parent = NbtTagEnumExtensions.IsCollection(((NbtNode)SelectedNode.Content).TagEnum)
            ? (NbtNode)SelectedNode.Content
            : (NbtNode)SelectedNode.Parent.Content;

        // 合法性判断
        if (parent.TagEnum == NbtTagEnum.List && parent.Tag.ChildrenTag != tagEnum)
        {
            await DialogService.ShowDialog("添加失败", "确认",
                description: $"列表<{parent.Tag.ChildrenTag}> 不允许添加 <{tagEnum}> 节点");
            return;
        }

        // 初始化弹窗
        var content = new AddNodeContent(tagEnum);
        var dialog = DialogService.GetDialog($"添加「{tagEnum}」节点", "确认", close: "取消", content: content);
        content.DialogOkButtonEnabledSetter = b => dialog.IsPrimaryButtonEnabled = b;
        dialog.IsPrimaryButtonEnabled = NbtTagEnumExtensions.IsCollection(tagEnum);

        // 获取输入的名称、值
        var choice = await dialog.ShowAsync();
        if (choice == ContentDialogResult.None) return;
        var name = content.NodeName;
        var value = content.NodeValue;

        // 一、向 NBT 标签实例添加节点
        var builder = new NbtTagBuilder(GameEdition == GameEdition.Java);
        var tag = tagEnum switch
        {
            NbtTagEnum.Byte => builder.Byte(name, byte.Parse((string)value)),
            NbtTagEnum.Short => builder.Short(name, short.Parse((string)value)),
            NbtTagEnum.Int => builder.Int(name, int.Parse((string)value)),
            NbtTagEnum.Long => builder.Long(name, long.Parse((string)value)),
            NbtTagEnum.Float => builder.Float(name, float.Parse((string)value)),
            NbtTagEnum.Double => builder.Double(name, double.Parse((string)value)),
            NbtTagEnum.ByteArray => builder.ByteArray(name, (byte[])value),
            NbtTagEnum.String => builder.String(name, (string)value),
            NbtTagEnum.List => builder.List(name, [], content.ChildrenTag),
            NbtTagEnum.Dictionary => builder.Dictionary(name, []),
            NbtTagEnum.IntArray => builder.IntArray(name, (int[])value),
            NbtTagEnum.LongArray => builder.LongArray(name, (long[])value),
            _ => throw new ArgumentOutOfRangeException(nameof(tagEnum), tagEnum, null)
        };
        parent.Tag.AppendChild(tag, []);

        // 二、向节点树添加节点(UI)
        parent.Children.Add(new NbtNode(tag));
        OnPropertyChanged(nameof(Nodes));

        if (FilePath != string.Empty) IsApplyEnabled = true;
        IsSaveEnabled = true;
    }
}