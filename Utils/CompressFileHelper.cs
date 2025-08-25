using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using NBT_Studio.Enum;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Size = SixLabors.ImageSharp.Size;

namespace NBT_Studio.Utils;

public static class CompressFileHelper
{
    /// <summary>
    ///     确认是否为 Gzip 或 Zlib 压缩文件
    /// </summary>
    /// <remarks>在方法执行完成后，文件流的位置将被重置</remarks>
    /// <param name="fileStream">文件流</param>
    /// <returns>是否为压缩文件、压缩文件类型</returns>
    public static (bool isCompressed, FileCompress? compressType) IsCompressedFile(Stream fileStream)
    {
        fileStream.Position = 0;
        var result = IsCompressed();
        fileStream.Position = 0;
        return result;

        (bool isCompressed, FileCompress? compressType) IsCompressed()
        {
            // 空文件
            if (fileStream.Length < 2) return (false, null);

            // 文件头
            var fileHead = new byte[2];
            fileStream.ReadExactly(fileHead, 0, 2);
            // Gzip 文件
            if (fileHead.SequenceEqual((byte[]) [0x1f, 0x8b])) return (true, FileCompress.Gzip);
            // Zlib 文件
            var cmf = fileHead[0];
            var flg = fileHead[1];
            if (((cmf << 8) | flg) % 31 == 0) return (true, FileCompress.Zlib);

            // 非压缩文件
            return (false, null);
        }
    }

    /// <summary>
    ///     解压一个 Gzip 或 Zlib 压缩文件
    /// </summary>
    /// <param name="fileStream">文件流</param>
    /// <param name="compressType">压缩文件类型</param>
    /// <remarks>在方法执行完成后，文件流的位置将被重置</remarks>
    /// <returns>文件的字节数组</returns>
    /// <exception cref="NotSupportedException">不支持的压缩类型</exception>
    public static byte[] DecompressFile(Stream fileStream, FileCompress compressType)
    {
        fileStream.Position = 0;
        using var memoryStream = new MemoryStream();
        using Stream compressStream = compressType switch
        {
            FileCompress.Gzip => new GZipStream(fileStream, CompressionMode.Decompress),
            FileCompress.Zlib => new ZLibStream(fileStream, CompressionMode.Decompress),
            _ => throw new NotSupportedException($"不支持解压[{compressType}]!")
        };

        compressStream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    /// <summary>
    ///     压缩图像
    /// </summary>
    /// <param name="image">图像</param>
    /// <param name="targetWidth">目标宽度</param>
    /// <param name="targetHeight">目标高度</param>
    /// <param name="cropRegion">裁切矩形(null表示不裁切)</param>
    public static void CompressImage(ref Image image, int targetWidth, int targetHeight,
        Rect? cropRegion = null)
    {
        var option = new ResizeOptions
        {
            Size = new Size(targetWidth, targetHeight),
            Mode = ResizeMode.Max
        };
        if (cropRegion != null)
            image.Mutate(x => x.Crop(new Rectangle(
                (int)cropRegion.Value.X,
                (int)cropRegion.Value.Y,
                (int)cropRegion.Value.Width,
                (int)cropRegion.Value.Height)));
        image.Mutate(x => x.Resize(option));
    }

    public static async Task CompressWriteBytes( Stream fileStream, byte[] data, FileCompress compressType)
    {
        await using Stream outStream = compressType switch
        {
            FileCompress.Gzip => new GZipStream(fileStream, CompressionMode.Compress),
            FileCompress.Zlib => new ZLibStream(fileStream, CompressionMode.Compress),
            _ => throw new NotSupportedException($"不支持对将文件压缩为[{compressType}]类型!")
        };
        await outStream.WriteAsync(data);
    }
}