using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using NBT_Studio.Enum.Settings;
using NBT_Studio.Message;
using NBT_Studio.Utils;

namespace NBT_Studio.ViewModel;

public partial class SettingsFlyoutViewModel : ObservableObject
{
    public SettingsFlyoutViewModel()
    {
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
            if (GetValueSetters().TryGetValue(key, out var value))
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
            { nameof(Theme), i => SelectedTheme = (Theme)i },
            { nameof(Sort), i => SelectedSort = (Sort)i }
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
    private static readonly Uri SettingsFileUri = new(AssetHelper.GetConfigUri());
    private Dictionary<string, int> _settings = new();

    #endregion

    #region Settings Properties

    private Sort _selectedSort;
    private Theme _selectedTheme;

    public Sort SelectedSort
    {
        get => _selectedSort;
        set
        {
            SetField(ref _selectedSort, value);
            WeakReferenceMessenger.Default.Send(new SettingsChangedMessage<Sort>(value));
            _ = UpdateSettingsFile();
        }
    }

    public Theme SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            SetField(ref _selectedTheme, value);
            WeakReferenceMessenger.Default.Send(new SettingsChangedMessage<Theme>(value));
            _ = UpdateSettingsFile();
        }
    }

    #endregion
}