using System.Collections.Generic;

namespace NBT_Studio.Model;

public class MinecraftItem
{
    public required int Count { get; set; }
    public required int Slot { get; set; }
    public required int Maximum { get; set; }
    public required string IdName { get; set; }
    public required string Name { get; set; }
    public List<MinecraftItem> SubItems { get; set; } = [];
    public string IconPath { get; set; } = "ms-appx:///Assets/ItemIcon/Empty.png";
}