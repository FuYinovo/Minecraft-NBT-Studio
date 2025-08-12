using NBT_Studio.Enum;
using NBT_Studio.Model;

namespace NBT_Studio.Message;

public class NodeChangedMessage((NbtNode node, NodeChangeType type) value)
{
    public readonly (NbtNode node, NodeChangeType type) Value = value;
}