using NBT_Studio.Enum;

namespace NBT_Studio.Xaml.Control.Dialog;

public sealed partial class SaveNodeDialog
{
    public SaveNodeDialog()
    {
        InitializeComponent();
    }

    public MinecraftEdition GameEdition { get; set; } = MinecraftEdition.Java;
    public FileCompress FileCompress { get; set; } = FileCompress.Gzip;
}