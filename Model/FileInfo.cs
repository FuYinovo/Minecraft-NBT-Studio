using NBT_Studio.Enum;

namespace NBT_Studio.Model;

public struct FileInfo
{
    public FileInfo()
    {
    }

    public string FilePath { get; set; } = string.Empty;
    public int FileLength { get; set; } = 0;
    public MinecraftEdition MinecraftEdition { get; set; } = MinecraftEdition.Unknown;
    public Endianness Endianness { get; set; } = Endianness.Unknown;
    public bool IsCompressed { get; set; } = false;
    public FileCompress CompressType { get; set; } = FileCompress.None;
}