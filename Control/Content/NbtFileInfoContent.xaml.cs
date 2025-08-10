using System;
using NBT_Studio.Enum;

namespace NBT_Studio.Control.Content;

public sealed partial class NbtFileInfoContent
{
    public NbtFileInfoContent(string filePath, int fileLength, GameEdition gameEdition, bool isBigEndian)
    {
        FilePath = filePath;
        FileLength = fileLength;
        GameEdition = gameEdition switch
        {
            Enum.GameEdition.Java => 0,
            Enum.GameEdition.Bedrock => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(gameEdition), gameEdition, null)
        };
        IsBigEndian = isBigEndian switch
        {
            true => 1,
            false => 0
        };
        InitializeComponent();
        switch (gameEdition)
        {
            case Enum.GameEdition.Bedrock:
                ItemJava.IsEnabled = false;
                break;
            case Enum.GameEdition.Java:
                ItemBedrock.IsEnabled = false;
                break;
            default: throw new Exception($"未知游戏版本[{gameEdition}]");
        }

        switch (isBigEndian)
        {
            case true:
                ItemLittleEndian.IsEnabled = false;
                break;
            case false:
                ItemBiggerEndian.IsEnabled = false;
                break;
        }
    }

    private string FilePath { get; }
    private int FileLength { get; }
    private int GameEdition { get; }
    private int IsBigEndian { get; }
}