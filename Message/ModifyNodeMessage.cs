using NBT_Studio.Enum;
using NBT_Studio.Model;

namespace NBT_Studio.Message;

public class ModifyNodeMessage((NbtNode node, NodeModify type) value)
{
    public readonly (NbtNode node, NodeModify type) Value = value;
}