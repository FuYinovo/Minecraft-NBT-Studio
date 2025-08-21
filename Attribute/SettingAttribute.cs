using System;
using NBT_Studio.Enum;

namespace NBT_Studio.Attribute;

[AttributeUsage(AttributeTargets.Field)]
public class SettingAttribute(object defaultValue, Type valueType) : System.Attribute
{
    public readonly object DefaultValue = defaultValue;
    public readonly Type ValueType = valueType;
}