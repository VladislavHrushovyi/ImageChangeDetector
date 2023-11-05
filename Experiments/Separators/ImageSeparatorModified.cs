using System.Drawing;
using System.Drawing.Imaging;

namespace Experiments.Separators;

public class ImageSeparatorModified : ITransformImage
{
    private readonly Bitmap _image;
    private readonly int _numberOfPixels;
    private readonly List<RowPointer> _rowPointers = new();
    public ImageSeparatorModified(Bitmap image)
    {
        _image = image;
        _numberOfPixels = image.Height * image.Width;
        _rowPointers.Add(new RowPointer(0, _numberOfPixels / 4));
    }
    public Bitmap Execute()
    {
        var height = _image.Height;
        var width = _image.Width;
        var resultImage = new Bitmap(width, height, _image.PixelFormat);

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
            _image.UnlockBits(bitmapDataOriginal);
            resultImage.UnlockBits(bitmapDataResult);
        }

        return resultImage;
    }

    class RowPointer
    {
        private int _startIndex;
        private readonly int _length;
        public RowPointer(int startIndex, int length)
        {
            _startIndex = startIndex - 1;
            _length = length;
        }

        public int GetCurrentPosition()
        {
            _startIndex++;
            if (_startIndex >= _length)
            {
                return _length - 1;
            }
            return _startIndex;
        }
    }
}