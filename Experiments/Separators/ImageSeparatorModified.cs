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
        InitializeRowPointers();
    }

    private void InitializeRowPointers()
    {
        int quarter = _numberOfPixels / 4;
        _rowPointers.Add(new RowPointer(0, quarter));
        _rowPointers.Add(new RowPointer(quarter, quarter * 2));
        _rowPointers.Add(new RowPointer(quarter * 2, quarter * 3));
        _rowPointers.Add(new RowPointer(quarter * 3, _numberOfPixels));
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
                    byte* currentLineOriginal = ptrOriginal + (y * strideOriginal);
                    byte* currentLineResult = ptrResult + (y * strideResult);
                    for (int x = 0; x < width - 4; x++)
                    {
                        for (int i = 0; i < _rowPointers.Count; i++)
                        {
                            int xOffset = _rowPointers[i].GetCurrentPosition();

                            int pixelOffset = (x+i) * bytesPerPixel;
                            for (int byteIndex = 0; byteIndex < bytesPerPixel; byteIndex++)
                            {
                                var currByte = currentLineOriginal[pixelOffset + byteIndex];
                                var temp = currentLineResult[xOffset + byteIndex];
                                currentLineResult[xOffset + byteIndex] = currByte;
                            }

                            _rowPointers[i].IncrementPosition(width);
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
}

class RowPointer
{
    private int _currentX;
    private readonly int _length;

    public RowPointer(int startIndex, int length)
    {
        _currentX = startIndex;
        _length = length;
    }

    public int GetCurrentPosition()
    {
        return _currentX;
    }

    public void IncrementPosition(int maxWidth)
    {
        _currentX++;
        if (_currentX >= _length || _currentX >= maxWidth)
        {
            _currentX = maxWidth - 1;
        }
    }
}