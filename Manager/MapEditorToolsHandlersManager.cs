using System;
using System.Collections.Generic;
using System.Reflection;
using NBT_Studio.Attribute;
using NBT_Studio.Enum;
using NBT_Studio.Handler;
using NBT_Studio.Interface;
using NBT_Studio.View;

namespace NBT_Studio.Manager;

public class MapEditorToolsHandlersManager(MapEditor editor)
{
    private readonly Dictionary<MapEditorTool, IMapEditorPointerEventsHandler> _handlers = [];

    public IMapEditorPointerEventsHandler GetHandler(MapEditorTool targetType)
    {
        if (_handlers.TryGetValue(targetType, out var handler)) return handler;
        _handlers[targetType] = GetHandlerFromAttribute(targetType);
        return _handlers[targetType];
    }

    private IMapEditorPointerEventsHandler GetHandlerFromAttribute(MapEditorTool targetType)
    {
        var members = typeof(MapEditorToolsHandlers).GetMembers();

        foreach (var member in members)
        {
            var toolType = member.GetCustomAttribute<MapEditorToolHandlerAttribute>()?.Tool;
            if (toolType != targetType) continue;
            // 此处默认 IMapEditorPointerEventsHandler 的实现者的构造方法只有一个参数 MapEditor
            var instance = Activator.CreateInstance((Type)member, editor);
            if (instance is IMapEditorPointerEventsHandler handler) return handler;
        }

        throw new Exception($"未找到地图编辑器[{targetType}]的Handler!");
    }
}