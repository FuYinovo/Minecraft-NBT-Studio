using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NBT_Studio.Service;

public static class DialogService
{
    /// <summary>
    ///     弹出一个对话框
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="primary">主按钮文本</param>
    /// <param name="secondary">次按钮文本</param>
    /// <param name="close">取消按钮文本</param>
    /// <param name="content">(优先)对话框自定义内容</param>
    /// <param name="description">对话框内容</param>
    /// <returns>用户选择的按钮</returns>
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

    /// <summary>
    ///     从获取主窗口 XamlRoot
    /// </summary>
    private static XamlRoot GetXamlRoot()
    {
        var xamlRoot = App.Window?.Content.XamlRoot;
        if (xamlRoot != null) return xamlRoot;
        throw new Exception("Failed to get XamlRoot");
    }
}