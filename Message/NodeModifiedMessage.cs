using NBT_Studio.Model;

namespace NBT_Studio.Message;

public class NodeModifiedMessage(NbtNode? node = null)
{
    public NbtNode? Node { get; } = node;
}