using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace NBT_Studio.Xaml.Control.Dialog;

public sealed partial class ResizeSquareImageDialog
{
    public ResizeSquareImageDialog(StorageFile image)
    {
        InitializeComponent();
        _ = LoadImage(image);
    }

    public Rect GetCroppedRegion()
    {
        return ImageCropper.CroppedRegion;
    }

    private async Task LoadImage(StorageFile image)
    {
        await ImageCropper.LoadImageFromFile(image);
    }
}