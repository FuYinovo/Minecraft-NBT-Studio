using Windows.Foundation;

namespace NBT_Studio.Message;

public class DetailPadSizeChangedMessage(Size value)
{
    public Size Value = value;
}