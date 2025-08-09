using System;
using NBT_Studio.Enum;

namespace NBT_Studio.Control.Content;

public sealed partial class RenameNodeContent
{
    public string NewName { get; set; } = string.Empty;

    public RenameNodeContent()
    {
        InitializeComponent();
    }
}