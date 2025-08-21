using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using NBT_Studio.Enum;
using NBT_Studio.Service;

namespace NBT_Studio.ViewModel;

public partial class SettingsFlyoutViewModel : ObservableObject
{
    private readonly SettingsManager _settingsManager = SettingsManager.GetInstance();

    public SettingsFlyoutViewModel()
    {
        _sortSetting = _settingsManager.GetValue<Sort>(EnumSettings.Sort);
        _themeSetting = _settingsManager.GetValue<Theme>(EnumSettings.Theme);
    }


    #region Properties

    private Sort _sortSetting;
    private Theme _themeSetting;


    public Sort SortSetting
    {
        get => _sortSetting;
        set
        {
            SetField(ref _sortSetting, value);
            _settingsManager.SetValue(EnumSettings.Sort, value);
        }
    }

    public Theme ThemeSetting
    {
        get => _themeSetting;
        set
        {
            SetField(ref _themeSetting, value);
            _settingsManager.SetValue(EnumSettings.Theme, value);
        }
    }

    #endregion

    # region INotifyPropertyChanged

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(propertyName);
    }

    #endregion NotifyPropertyChanged
}