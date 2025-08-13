using NBT_Studio.Enum;

namespace NBT_Studio.Control.Dialog;

public sealed partial class FileInfoContent
{
    public FileInfoContent(string filePath, int fileLength, GameEdition gameEdition, bool isBigEndian)
    {
        InitializeComponent();

        FilePath = filePath;
        FileLength = fileLength;
        GameEdition = gameEdition == Enum.GameEdition.Java ? 0 : 1;
        IsBigEndian = isBigEndian ? 1 : 0;
        ItemJava.IsEnabled = gameEdition == Enum.GameEdition.Java;
        ItemBedrock.IsEnabled = gameEdition == Enum.GameEdition.Bedrock;
        ItemBiggerEndian.IsEnabled = isBigEndian;
        ItemLittleEndian.IsEnabled = !isBigEndian;
    }

    private string FilePath { get; }
    private int FileLength { get; }
    private int GameEdition { get; }
    private int IsBigEndian { get; }
}