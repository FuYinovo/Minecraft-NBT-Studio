using System;

namespace NBT_Studio.Attribute;

[AttributeUsage(AttributeTargets.Field)]
public class SettingAttribute(object defaultValue, Type valueType) : System.Attribute
{
    public readonly Type ValueType = valueType;
    public readonly object DefaultValue = defaultValue;
}