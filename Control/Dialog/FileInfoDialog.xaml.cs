using NBT_Studio.Model;
using NBT_Studio.Utils;

namespace NBT_Studio.Control.Dialog;

public sealed partial class FileInfoDialog
{
    public FileInfoDialog(FileInfo fileInfo)
    {
        InitializeComponent();

        FilePath = fileInfo.FilePath;
        FileLength = fileInfo.FileLength;
        IsCompressed = fileInfo.IsCompressed;
        CompressType = IsCompressed ? fileInfo.CompressType.ToString() : string.Empty;
        MinecraftEdition = ReflectionHelper.GetEnumDescription(fileInfo.MinecraftEdition) ?? string.Empty;
        Endianness = ReflectionHelper.GetEnumDescription(fileInfo.Endianness) ?? string.Empty;
    }

    private string FilePath { get; }
    private int FileLength { get; }
    private string MinecraftEdition { get; }
    private string Endianness { get; }

    private bool IsCompressed { get; }
    private string CompressType { get; }
}