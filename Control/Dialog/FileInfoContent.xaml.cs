using NBT_Studio.Enum;

namespace NBT_Studio.Control.Dialog;

public sealed partial class FileInfoContent
{
    public FileInfoContent(string filePath, int fileLength, MinecraftEdition minecraftEdition, bool isBigEndian)
    {
        InitializeComponent();

        FilePath = filePath;
        FileLength = fileLength;
        MinecraftEdition = minecraftEdition == Enum.MinecraftEdition.Java ? 0 : 1;
        IsBigEndian = isBigEndian ? 1 : 0;
        ItemJava.IsEnabled = minecraftEdition == Enum.MinecraftEdition.Java;
        ItemBedrock.IsEnabled = minecraftEdition == Enum.MinecraftEdition.Bedrock;
        ItemBiggerEndian.IsEnabled = isBigEndian;
        ItemLittleEndian.IsEnabled = !isBigEndian;
    }

    private string FilePath { get; }
    private int FileLength { get; }
    private int MinecraftEdition { get; }
    private int IsBigEndian { get; }
}