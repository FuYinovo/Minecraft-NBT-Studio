using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace NBT_Studio.Control.MapEditorToggleButton;

public partial class MapEditorToggleSplitButton
{
    private const string DefaultSymbol = "{>}";
    private bool _addedDefault;

    public MapEditorToggleSplitButton()
    {
        InitializeComponent();
        Loaded += CreateMenuFlyoutItems;
    }

    private void CreateMenuFlyoutItems(object sender, RoutedEventArgs e)
    {
        foreach (var str in Menu)
        {
            var item = new RadioMenuFlyoutItem();
            var isDefault = str.Contains(DefaultSymbol);
            item.Text = isDefault ? str.Replace(DefaultSymbol, "") : str;
            if (isDefault && !_addedDefault)
            {
                item.IsChecked = true;
                _addedDefault = true;
            }

            MenuFlyout.Items.Add(item);
        }
    }

    /// <summary> 子菜单</summary>
    /// <remarks>默认选中包含 {>} 的子菜单（若有多个则取第一个、包含花括号）</remarks>
    public List<string> Menu
    {
        get => (List<string>)GetValue(_menuDp);
        set => SetValue(_menuDp, value);
    }

    private readonly DependencyProperty _menuDp = DependencyProperty.Register(
        nameof(Menu),
        typeof(List<string>),
        typeof(MapEditorToggleButton),
        new PropertyMetadata(new List<string>())
    );
}