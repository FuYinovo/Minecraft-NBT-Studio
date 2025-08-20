using System.Windows.Input;
using NBT_Studio.Library.NBT_Parser.Enum;
using NBT_Studio.Utils;

namespace NBT_Studio.Model;

public class CreateNodeButtonItem(NbtTagEnum tagEnum, ICommand? command = null)
{
    public readonly ICommand? Command = command;
    public readonly string IconUri = AssetHelper.GetNbtTagIconUri(tagEnum);
    public readonly NbtTagEnum Tag = tagEnum;
    public string Label = ReflectionHelper.GetEnumDescription(tagEnum) ?? tagEnum.ToString();
    public bool IsEnabled { get; set; } = true;
}