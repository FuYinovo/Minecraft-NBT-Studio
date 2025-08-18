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
        WeakReferenceMessenger.Default.Register<DetailPadSizeChangedMessage>(this,
            (_, v) => UpdateGridSize(v.Value));
    }

    // 自动调整内容 Grid 大小 ( CommunityToolKit 未实现 )
    private void UpdateGridSize(Size size)
    {
        if (size.Width <= 60 || size.Height <= 60) return;
        ControlGrid.Width = size.Width - 60;
        ControlGrid.Height = size.Height - 70;
    }

    // 禁止信息控件的改动
    private void CheckBox_Prohibit(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox checkBox) return;
        ControlProhibitHelper.DisableCheckBox(checkBox);
    }

    private void TextBox_OnValueChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        textBox.IsEnabled = !NbtTagEnumExtensions.IsCollection(_viewModel.TagEnum);
        if (textBox.FocusState == FocusState.Unfocused)
        {
            SetTextBoxIsValid(true);
            SetSaveButtonIsEnabled(false);
            return;
        }

        if (NbtTagEnumExtensions.IsArray(_viewModel.TagEnum))
        {
            if (textBox.DataContext is not NodeArrayElement arrayValue) return;
            var valueType = NbtTagEnumExtensions.GetArrayElementType(_viewModel.TagEnum);
            arrayValue.IsValid = NbtTagHelper.IsValueValid(valueType, textBox.Text);
            SetTextBoxIsValid(arrayValue.IsValid);
            SetSaveButtonIsEnabled(_viewModel.ArrayValues.Count(x => x.IsValid) == _viewModel.ArrayValues.Count);
        }
        else
        {
            var isValid = NbtTagHelper.IsValueValid(_viewModel.TagEnum, textBox.Text);
            SetTextBoxIsValid(isValid);
            SetSaveButtonIsEnabled(isValid);
        }


        return;

        void SetTextBoxIsValid(bool isValid)
        {
            textBox.BorderBrush = isValid ? BorderDefault.BorderBrush : BorderRed.BorderBrush;
        }

        void SetSaveButtonIsEnabled(bool isEnabled)
        {
            SaveButton.IsEnabled = isEnabled;
        }
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        SaveButton.IsEnabled = false;
    }

    private void TextBox_OnNameChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        if (textBox.FocusState != FocusState.Unfocused) SaveButton.IsEnabled = true;
    }

    private void RemoveArrayValue_OnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        _viewModel.RemoveArrayValueCommand.Execute(btn);
        SaveButton.IsEnabled = true;
    }

    private void AddArrayValue_OnClicked(object sender, RoutedEventArgs e)
    {
        _viewModel.AddArrayValueCommand.Execute(null);
        SaveButton.IsEnabled = false; // 避免空值保存
    }
}