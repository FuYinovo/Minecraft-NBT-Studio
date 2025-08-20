namespace NBT_Studio.Xaml.Control.Dialog;

public sealed partial class RenameNodeDialog
{
    public RenameNodeDialog()
    {
        InitializeComponent();
    }

    public string NewName { get; set; } = string.Empty;
}