using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NBT_Studio.Service;

public static class DialogService
{
    public static async Task<ContentDialogResult> ShowDialog(string title, string primary = "[null]",
        string secondary = "[null]",
        string close = "取消")
    {
        var xamlRoot = GetXamlRoot();
        var dialog = new ContentDialog
        {
            XamlRoot = xamlRoot,
            Title = title,
            PrimaryButtonText = primary == "[null]" ? null : primary,
            SecondaryButtonText = primary == "[null]" ? null : secondary,
            DefaultButton = ContentDialogButton.Primary,
            CloseButtonText = close,
        };

        return await dialog.ShowAsync();
    }

    private static XamlRoot GetXamlRoot()
    {
        var xamlRoot = App.Window?.Content.XamlRoot;
        if (xamlRoot != null) return xamlRoot;
        throw new Exception("Failed to get XamlRoot");
    }
}