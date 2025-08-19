using System.Collections.Generic;

namespace NBT_Studio.Model;

public record struct MinecraftItem
{
    public required int Count { get; set; }
    public required int Slot { get; init; }
    public required int Maximum { get; init; }
    public required string IdName { get; init; }
    public required string Name { get; set; }
    public List<MinecraftItem> SubItems { get; set; }
    public string IconPath { get; init; }
}