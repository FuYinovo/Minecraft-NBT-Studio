using NBT_Studio.Library.NBT_Parser.Enum;

namespace NBT_Studio.Utils;

public static class AssetHelper
{
    private const string UriHead = "ms-appx:///Assets/";
    private const string NbtTagIconPath = "NodeIcon/Data_node_";
    private const string NbtTagIconUriExtension = ".svg";

    /// <summary>
    /// 获取 ms-appx 前缀的节点图标路径
    /// </summary>
    /// <param name="tagEnum">节点类型</param>
    /// <returns>图标路径</returns>
    public static string GetNbtTagIconUri(NbtTagEnum tagEnum)
    {
        return tagEnum switch
        {
            NbtTagEnum.ByteArray => $"{UriHead}{NbtTagIconPath}byte-array{NbtTagIconUriExtension}",
            NbtTagEnum.IntArray => $"{UriHead}{NbtTagIconPath}int-array{NbtTagIconUriExtension}",
            NbtTagEnum.LongArray => $"{UriHead}{NbtTagIconPath}long-array{NbtTagIconUriExtension}",
            _ => $"{UriHead}{NbtTagIconPath}{tagEnum.ToString().ToLower()}{NbtTagIconUriExtension}"
        };
    }
}