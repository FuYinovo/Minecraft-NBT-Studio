using NBT_Studio.Model;

namespace NBT_Studio.Message;

public class NodeModifiedMessage(NbtNode node)
{
    public readonly NbtNode Value = node;
}