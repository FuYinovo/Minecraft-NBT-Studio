using NBT_Studio.Library.NBT_Parser.Enum;

namespace NBT_Studio.Utils;

public static class AssetHelper
{
    private const string UriHead = "ms-appx:///Assets/";

    /// <summary>
    ///     获取 ms-appx 前缀的节点图标路径
    /// </summary>
    /// <param name="tagEnum">节点类型</param>
    public static string GetNbtTagIconUri(NbtTagEnum tagEnum)
    {
        const string nbtTagIconPath = "NodeIcon/Data_node_";
        const string nbtTagIconUriExtension = ".svg";
        return tagEnum switch
        {
            NbtTagEnum.ByteArray => $"{UriHead}{nbtTagIconPath}byte-array{nbtTagIconUriExtension}",
            NbtTagEnum.IntArray => $"{UriHead}{nbtTagIconPath}int-array{nbtTagIconUriExtension}",
            NbtTagEnum.LongArray => $"{UriHead}{nbtTagIconPath}long-array{nbtTagIconUriExtension}",
            _ => $"{UriHead}{nbtTagIconPath}{tagEnum.ToString().ToLower()}{nbtTagIconUriExtension}"
        };
    }

    /// <summary>
    /// 获取配置文件路径
    /// </summary>
    public static string GetConfigUri()
    {
        return $"{UriHead}Config/Settings.json";
    }
}