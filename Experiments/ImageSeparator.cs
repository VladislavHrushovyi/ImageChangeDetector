using System.Drawing;

namespace Experiments;

public class ImageSeparator : ITransformImage
{
    private readonly Bitmap _originalImage;

    public ImageSeparator(Bitmap initImage)
    {
        _originalImage = initImage;
    }

    public Bitmap Execute()
    {
        var resultBitmap = new Bitmap(_originalImage.Width, _originalImage.Height);
        var originalPixelsChunked = _originalImage.AsStreamPixel().Chunk(4);
        foreach (var colorChunk in originalPixelsChunked)
        {
            
        }
        throw new NotImplementedException();
    }
}

class Pointer
{
    private readonly int Height;
    private readonly int Width;
    
    public Pointer(int width, int height)
    {
        Width = width;
        Height = height;
    }
    
    public 
}