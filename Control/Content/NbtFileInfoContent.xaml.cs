using System;
using NBT_Studio.Enum;

namespace NBT_Studio.Control.Content;

public sealed partial class NbtFileInfoContent
{
    private string FilePath { get; }
    private int FileLength { get; }
    private int GameEdition { get; }
    private int IsBigEndian { get; }

    public NbtFileInfoContent(string filePath, int fileLength, GameEditionEnum gameEditionEnum, bool isBigEndian)
    {
        FilePath = filePath;
        FileLength = fileLength;
        GameEdition = gameEditionEnum switch
        {
            GameEditionEnum.Java => 0,
            GameEditionEnum.Bedrock => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(gameEditionEnum), gameEditionEnum, null)
        };
        IsBigEndian = isBigEndian switch
        {
            true => 1,
            false => 0
        };
        InitializeComponent();
        switch (gameEditionEnum)
        {
            case GameEditionEnum.Bedrock:
                ItemJava.IsEnabled = false;
                break;
            case GameEditionEnum.Java:
                ItemBedrock.IsEnabled = false;
                break;
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
}