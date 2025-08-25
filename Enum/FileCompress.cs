using System.ComponentModel;

namespace NBT_Studio.Enum;

public enum FileCompress
{
    Gzip,
    Zlib,
    [Description("不压缩")]None
}