using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using NBT_Studio.Enum.Settings;
using NBT_Studio.Message;

namespace NBT_Studio.ViewModel;

public partial class SettingsFlyoutViewModel : ObservableObject
{
    public SettingsFlyoutViewModel()
    {
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
    ///     获取「名称」到「[方法]设置值」的字典
    /// </summary>
    private Dictionary<string, Action<int>> GetValueSetters()
    {
        return new Dictionary<string, Action<int>>
        {
            { nameof(Theme), v => Theme = (Theme)v },
            { nameof(Sort), v => Sort = (Sort)v }
        };
    }


    #region Private Properties

    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };
    private static readonly Uri SettingsFileUri = new("ms-appx:///Assets/Config/Settings.json");
    private Dictionary<string, int> _settings = new();
    private readonly Dictionary<string, Action<int>> _nameToValueSetter;

    #endregion

    #region Settings Properties

    private Sort _sort = Sort.Default;
    private Theme _theme = Theme.Default;

    public Sort Sort
    {
        get => _sort;
        set => SetFieldForSettings(ref _sort, value, Sort);
    }

    public Theme Theme
    {
        get => _theme;
        set => SetFieldForSettings(ref _theme, value, Theme);
    }

    #endregion

    # region INotifyPropertyChanged

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(propertyName);
    }

    private void SetFieldForSettings<T, TE>(ref T field, T value, TE settingsEnum,
        [CallerMemberName] string? propertyName = null) where TE : System.Enum
    {
        SetField(ref field, value, propertyName);
        WeakReferenceMessenger.Default.Send(new SettingsValueChangedMessage<TE>(settingsEnum));
        _ = UpdateSettingsFile();
    }

    #endregion NotifyPropertyChanged
}