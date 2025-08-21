using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using NBT_Studio.Enum;
using NBT_Studio.Service;
using Vanara.PInvoke;

namespace NBT_Studio.ViewModel;

public partial class SettingsFlyoutViewModel : ObservableObject
{
    private readonly SettingsManager _settingsManager = SettingsManager.GetInstance();

    public SettingsFlyoutViewModel()
    {
        _sort = _settingsManager.GetValue<Sort>(EnumSettings.Sort);
        _theme = _settingsManager.GetValue<Theme>(EnumSettings.Theme);
    }


    #region Properties

    private Sort _sort;
    private Theme _theme;
    private bool _quickCreate;


    public Sort Sort
    {
        get => _sort;
        set => SetSetting(ref _sort, value, EnumSettings.Sort);
    }

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
        _settingsManager.SetValue(settingEnum, value);
    }

    #endregion NotifyPropertyChanged
}