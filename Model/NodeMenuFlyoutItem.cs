using System.Windows.Input;
using Windows.System;
using NBT_Studio.Library.NBT_Parser.Enum;

namespace NBT_Studio.Model;

public struct NodeMenuFlyoutItem
{
    public NodeMenuFlyoutItem(NbtTagEnum commandParameter,
        VirtualKey key = VirtualKey.None,
        VirtualKeyModifiers modifier = VirtualKeyModifiers.None, ICommand? command = null)
    {
        Title = Utils.ReflectionHelper.GetEnumDescription(commandParameter) ?? commandParameter.ToString();
        Command = command;
        CommandParameter = commandParameter;
        IsSeparator = false;
        Key = key;
        ModifierKey = modifier;
    }

    public static NodeMenuFlyoutItem Separator = new() { IsSeparator = true };
    public readonly string Title;
    public ICommand? Command;
    public readonly NbtTagEnum CommandParameter;
    public bool IsSeparator = false;
    public readonly VirtualKey Key = VirtualKey.None;
    public readonly VirtualKeyModifiers ModifierKey = VirtualKeyModifiers.None;
}