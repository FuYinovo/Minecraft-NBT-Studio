using NBT_Studio.Library.NBT_Parser.Enum;
using NBT_Studio.Model;

namespace NBT_Studio.Message;

public class CreateNodeMessage(NbtNode invokedNode, NbtTagEnum tagEnum)
{
    public (NbtNode invokedNode, NbtTagEnum tagEnum) Value = (invokedNode, tagEnum);
}