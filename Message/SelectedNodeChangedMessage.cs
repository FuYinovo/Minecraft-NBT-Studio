
using Microsoft.UI.Xaml.Controls;

namespace NBT_Studio.Message;

public class SelectedNodeChangedMessage(TreeViewNode value)
{
    public readonly TreeViewNode Value = value;
}