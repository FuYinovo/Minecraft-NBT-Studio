using System.Linq;
using Windows.Foundation;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NBT_Studio.Library.NBT_Parser.Enum;
using NBT_Studio.Message;
using NBT_Studio.Model;
using NBT_Studio.Utils;
using NBT_Studio.ViewModel;

namespace NBT_Studio.View;

public sealed partial class ValueEditor
{
    private readonly ValueEditorViewModel _viewModel = new();

    public ValueEditor()
    {
        InitializeComponent();
        RegisterMessages();
    }

    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<DetailPadSizeChangedMessage>(this, DetailPadSizeChanged);
        WeakReferenceMessenger.Default.Register<SelectedNodeChangedMessage>(this, SelectedNodeChanged);

        return;

        void DetailPadSizeChanged(object sender, DetailPadSizeChangedMessage msg)
        {
            UpdateGridSize(msg.Value);
        }

        void SelectedNodeChanged(object sender, SelectedNodeChangedMessage msg)
        {
            var node = (NbtNode)msg.Value.Content;
            GenericValue.IsEnabled = !NbtTagEnumExtensions.IsCollection(node.TagEnum);
        }
    }

    /// <summary> 自动调整内容 Grid 大小 </summary>
    private void UpdateGridSize(Size size)
    {
        if (size.Width <= 60 || size.Height <= 60) return;
        ControlGrid.Width = size.Width - 60;
        ControlGrid.Height = size.Height - 70;
    }

    /// <summary> 禁止展示区勾选框的改动 </summary>
    private void CheckBox_Prohibit(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox)
            ControlProhibitHelper.DisableCheckBox(checkBox);
    }

    /// <summary> 当节点值改动 </summary>
    private void TextBox_OnValueChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        var text = textBox.Text;
        var tagEnum = _viewModel.TagEnum;

        // 容器标签没有值
        if (NbtTagEnumExtensions.IsCollection(tagEnum))
        {
            SetTextBoxStatus(textBox, true, false);
            return;
        }

        // 忽略更新节点详情而触发的 OnChanged
        if (textBox.FocusState == FocusState.Unfocused)
        {
            SetTextBoxStatus(textBox, true);
            SetSaveButtonStatus(false);
            return;
        }

        // 若为数组元素文本框
        if (NbtTagEnumExtensions.IsArray(tagEnum))
        {
            if (textBox.DataContext is not NodeArrayElement arrayValue) return;
            var valueType = NbtTagEnumExtensions.GetArrayElementType(tagEnum);
            arrayValue.IsValid = NbtTagHelper.IsValueValid(valueType, text);
            SetTextBoxStatus(textBox, arrayValue.IsValid, true);
            SetSaveButtonStatus(IsAllArrayValuesValid());
        }
        // 若为普通元素文本框
        else
        {
            var isValid = NbtTagHelper.IsValueValid(tagEnum, text);
            SetTextBoxStatus(textBox, isValid, true);
            SetSaveButtonStatus(isValid);
        }
    }

    /// <summary> 当点击保存按钮 </summary>
    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        SetSaveButtonStatus(false);
    }

    /// <summary> 当节点名称改动 </summary>
    private void TextBox_OnNameChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox && textBox.FocusState != FocusState.Unfocused)
            SetSaveButtonStatus(true);
    }

    /// <summary> 当尝试移除某数组元素 </summary>
    private void RemoveArrayValue_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        _viewModel.RemoveArrayValueCommand.Execute(btn);
        SetSaveButtonStatus(IsAllArrayValuesValid());
    }

    /// <summary> 当尝试添加一个数组元素 </summary>
    private void AddArrayValue_OnClicked(object sender, RoutedEventArgs e)
    {
        _viewModel.AddArrayValueCommand.Execute(null);
        SetSaveButtonStatus(false); // 避免空值保存
    }

    /// <summary> 设置文本框合法性 </summary>
    private void SetTextBoxStatus(TextBox textBox, bool isValid, bool? isEnabled = null)
    {
        textBox.BorderBrush = isValid ? BorderDefault.BorderBrush : BorderRed.BorderBrush;
        textBox.IsEnabled = isEnabled ?? isValid;
    }

    /// <summary> 设置保存按钮合法性（IsEnabled） </summary>
    private void SetSaveButtonStatus(bool isValid)
    {
        SaveButton.IsEnabled = isValid;
    }

    /// <summary> 是否所有数组元素均合法 </summary>
    private bool IsAllArrayValuesValid()
    {
        return _viewModel.ArrayValues.Count(x => x.IsValid) == _viewModel.ArrayValues.Count;
    }
}