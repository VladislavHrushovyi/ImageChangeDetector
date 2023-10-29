using System.Drawing;

namespace Experiments;

public class ImageSeparator : ITransformImage
{
    private readonly Bitmap _originalImage;
    private readonly List<Pointer> _separatorPointers = new();
    public ImageSeparator(Bitmap initImage)
    {
        _originalImage = initImage;
        _separatorPointers.Add(new Pointer(0, 0, _originalImage.Width / 2, _originalImage.Height / 2));
        _separatorPointers.Add(new Pointer(_originalImage.Width / 2, 0, _originalImage.Width, _originalImage.Height / 2));
        _separatorPointers.Add(new Pointer(0, _originalImage.Height / 2 - 1, _originalImage.Width / 2, _originalImage.Height));
        _separatorPointers.Add(new Pointer(_originalImage.Width / 2, _originalImage.Height / 2 - 1, _originalImage.Width, _originalImage.Height));
    }

    public Bitmap Execute()
    {
        var resultBitmap = new Bitmap(_originalImage.Width, _originalImage.Height);
        var originalPixelsChunked = _originalImage.AsStreamPixel().Chunk(4);
        foreach (var colorChunk in originalPixelsChunked)
        {
            for (int i = 0; i < colorChunk.Length; i++)
            {
                var position = _separatorPointers[i].GetCurrPosition();
                resultBitmap.SetPixel(position.Item2, position.Item1, colorChunk[i]);
            }
        }

        return resultBitmap;
    }
}

class Pointer
{
    private readonly int _startX;
    private readonly int _startY;
    private int _x;
    private int _y;
    private readonly int _height;
    private readonly int _width;
    
    public Pointer(int startX, int startY, int width, int height)
    {
        this._startX = startX;
        this._startY = startY;
        _x = startX;
        _y = startY;
        _width = width;
        _height = height;
    }

    public (int, int) GetCurrPosition()
    {
        _x++;
        if (_x == _width)
        {
            _x = _startX;
            _y++;
        }
        if (_startX > _width || _startY > _height)
        {
            throw new ArgumentOutOfRangeException();
        }

        return (_y, _x);
    }
}