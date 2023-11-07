using System.Drawing;
using System.Drawing.Imaging;

namespace Experiments.Separators;

public class ImageSeparatorModified : ITransformImage
{
    private readonly Bitmap _image;
    private readonly int _totalBytes;
    private readonly List<RowPointer> _rowPointers = new();
    public ImageSeparatorModified(Bitmap image)
    {
        _image = image;
        _totalBytes = (image.Height * image.Width) * 3;
        InitializeRowPointers();
    }

    private void InitializeRowPointers()
    {
        int quarter = _totalBytes / 4;
        _rowPointers.Add(new RowPointer(0, quarter));
        _rowPointers.Add(new RowPointer(quarter, quarter * 2));
        _rowPointers.Add(new RowPointer(quarter * 2, quarter * 3));
        _rowPointers.Add(new RowPointer(quarter * 3, _totalBytes));
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
                byte* ptrOriginal = (byte*)bitmapDataOriginal.Scan0;
                byte* ptrResult = (byte*)bitmapDataResult.Scan0;

                for (int i = 0; i < _totalBytes - 12; i += 12)
                {
                    for (int p = 0; p < _rowPointers.Count; p++)
                    {
                        var position = _rowPointers[p].GetCurrentPosition();
                        for (int pixelByte = 0; pixelByte < bytesPerPixel; pixelByte++)
                        {
                            ptrResult[position + pixelByte] = ptrOriginal[p * 3 + i + pixelByte];
                        }
                        _rowPointers[p].IncrementPosition(); 
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
    private int _currPosition;
    private readonly int _endPosition;

    public RowPointer(int startIndex, int endPosition)
    {
        _currPosition = startIndex;
        _endPosition = endPosition * 3;
    }

    public int GetCurrentPosition()
    {
        return _currPosition;
    }

    public void IncrementPosition()
    {
        _currPosition+=3;
        if (_currPosition >= _endPosition)
        {
            _currPosition = _endPosition - 3;
        }
    }
}
