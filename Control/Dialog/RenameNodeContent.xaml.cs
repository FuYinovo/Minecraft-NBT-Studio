namespace NBT_Studio.Control.Dialog;

public sealed partial class RenameNodeContent
{
    public RenameNodeContent()
    {
        InitializeComponent();
    }

    public string NewName { get; set; } = string.Empty;
}