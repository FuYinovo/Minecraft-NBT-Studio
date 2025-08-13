using Windows.Foundation;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using NBT_Studio.Library.NBT_Parser.Enum;
using NBT_Studio.Message;
using NBT_Studio.Utils;
using NBT_Studio.ViewModel;

namespace NBT_Studio.Control;

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
        WeakReferenceMessenger.Default.Register<NodeValuePanelSizeChangedMessage>(this,
            (_, v) => UpdateGridSize(v.Value));
    }

    // 自动调整内容 Grid 大小 ( CommunityToolKit 未实现 )
    private void UpdateGridSize(Size size)
    {
        if (size.Width <= 60) return;
        ControlGrid.Width = size.Width - 60;
        ControlGrid.Height = size.Height - 70;
    }

    // 禁止信息控件的改动
    private void CheckBox_Prohibit(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox checkBox) return;
        ControlDisabler.DisableCheckBox(checkBox);
    }

    private void TextBox_OnValueChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        textBox.IsEnabled = !NbtTagEnumExtensions.IsCollection(_viewModel.TagEnum);
        if (textBox.FocusState == FocusState.Unfocused)
        {
            SetState(false, BorderDefault.BorderBrush);
            return;
        }

        SetState(NbtTagValueChecker.IsValid(_viewModel.TagEnum, textBox.Text));


        return;

        // 设置「保存」按钮启用状态、「输入框」边框笔刷
        void SetState(bool isEnabled, Brush? borderBrush = null)
        {
            SaveButton.IsEnabled = isEnabled;
            textBox.BorderBrush = borderBrush ?? (isEnabled ? BorderDefault.BorderBrush : BorderRed.BorderBrush);
        }
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        SaveButton.IsEnabled = false;
    }
}