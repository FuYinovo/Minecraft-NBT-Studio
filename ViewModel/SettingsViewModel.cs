using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.UI.Xaml;
using NBT_Studio.Enum;

namespace NBT_Studio.ViewModel;

public sealed partial class SettingsViewModel : INotifyPropertyChanged
{
    public SettingsViewModel(TreeViewPageViewModel pageViewModel)
    {
        _pageViewModel = pageViewModel;
        _nameToValueSetter = GetValueSetters();
        _ = LoadSettingsFile();
    }

    /// <summary>
    /// 加载配置文件
    /// </summary>
    private async Task LoadSettingsFile()
    {
        // 读取配置文件
        var filePath = (await StorageFile.GetFileFromApplicationUriAsync(SettingsFileUri)).Path;
        var jsonCent = await File.ReadAllTextAsync(filePath);
        _settings = JsonSerializer.Deserialize<Dictionary<string, int>>(jsonCent, _jsonSerializerOptions) ?? [];
        // 还原值
        foreach (var key in _settings.Keys)
        {
            if (_nameToValueSetter.TryGetValue(key, out var value)) value(_settings[key]);
        }
        Debug.WriteLine(Theme + " : " + SortType);
    }

    /// <summary>
    /// 更新配置文件
    /// </summary>
    private async Task UpdateSettingsFile()
    {
        var filePath = (await StorageFile.GetFileFromApplicationUriAsync(SettingsFileUri)).Path;
        var jsonContent = JsonSerializer.Serialize(_settings, _jsonSerializerOptions);
        await File.WriteAllTextAsync(filePath, jsonContent);
    }

    /// <summary>
    /// 获取「名称」到「[方法]设置值」的字典
    /// </summary>
    private  Dictionary<string, Action<int>> GetValueSetters()
    {
        return new Dictionary<string, Action<int>>
        {
            { nameof(Theme), v => Theme = (Theme)v },
            { nameof(SortType), v => SortType = (SortType)v },
        };
    }


    #region ApplySettings

    private static void ApplyTheme(Theme value)
    {
        if (App.MainWindow == null || App.MainWindow.Content is not FrameworkElement window) return;

        window.RequestedTheme = value switch
        {
            Theme.Default => ElementTheme.Default,
            Theme.Dark => ElementTheme.Dark,
            Theme.Light => ElementTheme.Light,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    private void ApplySort(SortType value)
    {
        _pageViewModel.ApplySort(value);
    }

    #endregion

    #region Private Properties

    private readonly TreeViewPageViewModel _pageViewModel;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };
    private static readonly Uri SettingsFileUri = new("ms-appx:///Assets/Config/Settings.json");
    private SortType _sortType = SortType.Default;
    private Theme _theme = Theme.Default;
    private Dictionary<string, int> _settings = new();
    private readonly Dictionary<string, Action<int>> _nameToValueSetter;

    #endregion

    #region Public Properties

    public SortType SortType
    {
        get => _sortType;
        set
        {
            SetField(ref _sortType, value);
            _ = UpdateSettingsFile();
            ApplySort(value);
        }
    }

    public Theme Theme
    {
        get => _theme;
        set
        {
            SetField(ref _theme, value);
            _ = UpdateSettingsFile();
            ApplyTheme(value);
        }
    }


    #endregion

    #region INotifyPropertyChanged

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

    #endregion INotifyPropertyChanged
}