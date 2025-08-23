using System;
using NBT_Studio.Attribute;
using NBT_Studio.Interface;

namespace NBT_Studio.Model;

public class Setting<T>(SettingAttribute attribute) : ISetting
{
    private T _value = (T)attribute.DefaultValue;

    public object Value
    {
        get => _value ?? throw new NullReferenceException($"尝试获取[{Attribute.ValueType}]的值，但是返回了[null]!");
        set => _value = (T)value;
    }

    public SettingAttribute Attribute { get; } = attribute;
}