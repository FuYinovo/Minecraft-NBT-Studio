using NBT_Studio.Attribute;
using NBT_Studio.Interface;

namespace NBT_Studio.Model;

public class Setting<T>(SettingAttribute attribute) : ISetting
{
    public object Value
    {
        get => _value;
        set => _value = (T)value;
    }

    public SettingAttribute Attribute { get; } = attribute;
    private T _value = (T)attribute.DefaultValue;
}