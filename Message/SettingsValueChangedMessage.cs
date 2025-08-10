namespace NBT_Studio.Message;

public class SettingsValueChangedMessage<T>(T value)
    where T : System.Enum
{
    public readonly T Value = value;
}