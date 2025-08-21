using System;

namespace NBT_Studio.Message;

public class SettingsChangedMessage(Type valueType, object newValue)
{
    public readonly Type ValueType = valueType;
    public readonly object NewValue = newValue;
}