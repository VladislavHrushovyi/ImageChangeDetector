using System.Drawing;
using System.Drawing.Imaging;

namespace Experiments.Separators;

public class ImageSeparatorModified : ITransformImage
{
    private readonly Bitmap _image;
    public ImageSeparatorModified(Bitmap image)
    {
        _image = image;
    }
    public Bitmap Execute()
    {
        var height = _image.Height;
        var width = _image.Width;
        var resultImage = new Bitmap(width, height, _image.PixelFormat);

        // Lock the bitmaps
        BitmapData bitmapDataOriginal = _image.LockBits(new Rectangle(0, 0, width, height),
            ImageLockMode.ReadOnly,
            _image.PixelFormat);
        BitmapData bitmapDataResult = resultImage.LockBits(new Rectangle(0, 0, width, height),
            ImageLockMode.WriteOnly,
            resultImage.PixelFormat);

        try
        {
            unsafe
            {
                int bytesPerPixel = Image.GetPixelFormatSize(_image.PixelFormat) / 8;
                int strideOriginal = bitmapDataOriginal.Stride;
                int strideResult = bitmapDataResult.Stride;
                byte* ptrOriginal = (byte*)bitmapDataOriginal.Scan0;
                byte* ptrResult = (byte*)bitmapDataResult.Scan0;

                // Copy the RGB values from original to result image
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte* currentLineOriginal = ptrOriginal + (y * strideOriginal);
                        byte* currentLineResult = ptrResult + (y * strideResult);
                        int pixelOffset = x * bytesPerPixel;
                        for (int byteIndex = 0; byteIndex < bytesPerPixel; byteIndex++)
                        {
                            currentLineResult[pixelOffset + byteIndex] = currentLineOriginal[pixelOffset + byteIndex];
                        }
                    }
                }
            }
        }
        finally
        {
            // Unlock the bits for both original and result images
            _image.UnlockBits(bitmapDataOriginal);
            resultImage.UnlockBits(bitmapDataResult);
        }

        return resultImage;
    }
}