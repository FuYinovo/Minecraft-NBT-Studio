using System;
using NBT_Studio.Attribute;

namespace NBT_Studio.Interface;

public interface ISetting
{
    SettingAttribute Attribute { get; }
    object Value { get; set; }
    public void Reset() => Value = Attribute.DefaultValue;
}