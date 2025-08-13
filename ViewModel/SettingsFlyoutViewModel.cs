using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Enum.Settings;
using NBT_Studio.Message;

namespace NBT_Studio.ViewModel;

public partial class SettingsFlyoutViewModel : ObservableObject
{
    public SettingsFlyoutViewModel()
    {
        _selectedSort = SortItems[0];
        _selectedTheme = ThemeItems[0];
        _nameToValueSetter = GetValueSetters();
        _ = LoadSettingsFile();
    }

    /// <summary>
    ///     加载配置文件
    /// </summary>
    private async Task LoadSettingsFile()
    {
        // 读取配置文件
        var filePath = (await StorageFile.GetFileFromApplicationUriAsync(SettingsFileUri)).Path;
        var jsonCent = await File.ReadAllTextAsync(filePath);
        _settings = JsonSerializer.Deserialize<Dictionary<string, int>>(jsonCent, _jsonSerializerOptions) ?? [];
        // 还原值
        foreach (var key in _settings.Keys)
            if (_nameToValueSetter.TryGetValue(key, out var value))
                value(_settings[key]);
    }

    /// <summary>
    ///     更新配置文件
    /// </summary>
    private async Task UpdateSettingsFile()
    {
        var filePath = (await StorageFile.GetFileFromApplicationUriAsync(SettingsFileUri)).Path;
        var jsonContent = JsonSerializer.Serialize(_settings, _jsonSerializerOptions);
        await File.WriteAllTextAsync(filePath, jsonContent);
    }

    /// <summary>
    ///     获取「名称」到「方法:设置值」的字典
    /// </summary>
    private Dictionary<string, Action<int>> GetValueSetters()
    {
        return new Dictionary<string, Action<int>>
        {
            { nameof(Theme), i => SelectedTheme = ThemeItems[i] },
            { nameof(Sort), i => SelectedSort = SortItems[i] }
        };
    }

    # region INotifyPropertyChanged

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(propertyName);
    }

    #endregion NotifyPropertyChanged


    #region Private Properties

    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };
    private static readonly Uri SettingsFileUri = new("ms-appx:///Assets/Config/Settings.json");
    private Dictionary<string, int> _settings = new();
    private readonly Dictionary<string, Action<int>> _nameToValueSetter;

    #endregion

    #region Settings Properties

    private ComboBoxItem _selectedSort;
    private ComboBoxItem _selectedTheme;

    public ComboBoxItem SelectedSort
    {
        get => _selectedSort;
        set
        {
            SetField(ref _selectedSort, value);
            WeakReferenceMessenger.Default.Send(new SettingsChangedMessage<Sort>((Sort)value.Tag));
            _ = UpdateSettingsFile();
        }
    }

    public ComboBoxItem SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            SetField(ref _selectedTheme, value);
            WeakReferenceMessenger.Default.Send(new SettingsChangedMessage<Theme>((Theme)value.Tag));
            _ = UpdateSettingsFile();
        }
    }

    #endregion

    # region Settings Items

    public List<ComboBoxItem> SortItems { get; } =
    [
        new() { Tag = Sort.Default, Content = "默认" },
        new() { Tag = Sort.Alphabetical, Content = "首字母" },
        new() { Tag = Sort.ByType, Content = "类型" }
    ];

    public List<ComboBoxItem> ThemeItems { get; } =
    [
        new() { Tag = Theme.Default, Content = "系统" },
        new() { Tag = Theme.Dark, Content = "深色" },
        new() { Tag = Theme.Light, Content = "浅色" }
    ];

    #endregion
}