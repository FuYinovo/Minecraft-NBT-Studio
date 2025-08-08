using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NBT_Studio.Service;

public static class DialogService
{
    public static async Task<ContentDialogResult> ShowDialog(string title, string? primary = null,
        string? secondary = null,
        string? close = null, Microsoft.UI.Xaml.Controls.Control? content = null, string? description = null)
    {
        var xamlRoot = GetXamlRoot();
        var dialog = new ContentDialog
        {
            XamlRoot = xamlRoot,
            Title = title,
            PrimaryButtonText = primary,
            SecondaryButtonText = secondary,
            CloseButtonText = close,
            DefaultButton = ContentDialogButton.Primary,
            Content = content == null
                ? new ContentControl { Content = new Grid { Children = { new TextBlock { Text = description } } } }
                : content
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