using System;
using System.Reflection;
using NBT_Studio.Attribute;
using NBT_Studio.Enum;
using NBT_Studio.Handler;
using NBT_Studio.Interface;
using NBT_Studio.View;

namespace NBT_Studio.Manager;

public class MapEditorToolsHandlersManager(MapEditor editor)
{
    public IPointerEventsHandler GetHandler(MapEditorTool targetType)
    {
        var members = typeof(MapEditorToolsHandlers).GetMembers();

        foreach (var member in members)
        {
            var toolType = member.GetCustomAttribute<MapEditorToolHandlerAttribute>()?.Tool;
            if (toolType != targetType) continue;
            // 此处默认 IPointerEventsHandler 的实现者的构造方法只有一个参数 MapEditor
            var instance = Activator.CreateInstance((Type)member, editor);
            if (instance is IPointerEventsHandler handler) return handler;
        }

        throw new Exception($"未找到地图编辑器[{targetType}]的Handler!");
    }
}