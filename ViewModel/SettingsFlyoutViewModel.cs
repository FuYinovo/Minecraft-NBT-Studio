using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using NBT_Studio.Enum;
using NBT_Studio.Service;

namespace NBT_Studio.ViewModel;

public partial class SettingsFlyoutViewModel : ObservableObject
{
    private readonly SettingsService _settingsService = SettingsService.GetInstance();

    public SettingsFlyoutViewModel()
    {
        _theme = _settingsService.GetValue<Theme>(EnumSettings.Theme);
        _quickCreate = _settingsService.GetValue<bool>(BooleanSettings.QuickCreate);
    }


    #region Properties

    private Theme _theme;
    private bool _quickCreate;

    public Theme Theme
    {
        get => _theme;
        set => SetSetting(ref _theme, value, EnumSettings.Theme);
    }

    public bool QuickCreate
    {
        get => _quickCreate;
        set => SetSetting(ref _quickCreate, value, BooleanSettings.QuickCreate);
    }

    #endregion

    # region INotifyPropertyChanged

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(propertyName);
    }

    private void SetSetting<T, TE>(ref T field, T value, TE settingEnum, [CallerMemberName] string? propertyName = null)
        where TE : System.Enum
        where T : IConvertible
    {
        SetField(ref field, value, propertyName);
        _settingsService.SetValue(settingEnum, value);
    }

    #endregion NotifyPropertyChanged
}