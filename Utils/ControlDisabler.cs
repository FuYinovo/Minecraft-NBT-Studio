using Microsoft.UI.Xaml.Controls;

namespace NBT_Studio.Utils;

/// <summary>
/// 提供「禁用控件交互，但不影响样式」的方法
/// </summary>
public static class ControlDisabler
{
    /// <summary>
    /// 遍历下拉框每个选项，禁用「枚举标签」与「目标枚举」不一致的选项
    /// </summary>
    /// <remarks>用于「DropDownOpened」事件</remarks>
    /// <param name="comboBox">下拉框</param>
    /// <param name="sourceEnum">目标枚举</param>
    public static void DisableComboBoxItemsByTag<T>(ComboBox comboBox, T sourceEnum) where T: System.Enum
    {
        foreach (var item in comboBox.Items)
        {
            if(item is not ComboBoxItem { Tag: T itemEnum } comboBoxItem) return;
            comboBoxItem.IsEnabled = itemEnum.ToString() == sourceEnum.ToString();
        }
    }

    /// <summary>
    /// 将单选框的勾选状态取反
    /// </summary>
    /// <remarks>用于「Click」事件</remarks>
    /// <param name="checkBox">勾选框</param>
    public static void DisableCheckBox(CheckBox checkBox)
    {
        checkBox.IsChecked = !checkBox.IsChecked;
    }
}