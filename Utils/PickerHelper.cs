using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace NBT_Studio.Utils;

public static class PickerHelper
{
    public static async Task<StorageFile?> PickFileAsync(string[] fileTypes)
    {
        var picker = new FileOpenPicker
        {
            ViewMode = PickerViewMode.Thumbnail
        };
        foreach (var type in fileTypes) picker.FileTypeFilter.Add(type);
        var hWnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(picker, hWnd);
        return await picker.PickSingleFileAsync();
    }

    public static async Task<StorageFile?> SaveFileAsync(IDictionary<string, IList<string>> fileTypes,
        string suggestedName)
    {
        var picker = new FileSavePicker
        {
            SuggestedFileName = suggestedName
        };
        foreach (var type in fileTypes) picker.FileTypeChoices.Add(type);
        var hWnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(picker, hWnd);
        return await picker.PickSaveFileAsync();
    }
}