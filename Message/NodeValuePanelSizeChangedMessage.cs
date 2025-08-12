using Windows.Foundation;

namespace NBT_Studio.Message;

public class NodeValuePanelSizeChangedMessage(Size value)
{
    public Size Value = value;
}