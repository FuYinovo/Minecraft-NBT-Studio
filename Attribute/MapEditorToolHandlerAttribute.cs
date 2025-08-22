using System;
using NBT_Studio.Enum;

namespace NBT_Studio.Attribute;

[AttributeUsage(AttributeTargets.Class)]
public class MapEditorToolHandlerAttribute : System.Attribute
{
    public MapEditorTool Tool;
}