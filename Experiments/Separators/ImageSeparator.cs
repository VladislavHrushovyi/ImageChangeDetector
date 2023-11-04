using System.Drawing;
using ImageChangeDetector;

namespace Experiments.Separators;

public class ImageSeparator : ITransformImage
{
    private readonly IMatrixAccessor _originalImage;
    private readonly List<Pointer> _separatorPointers = new();
    public ImageSeparator(IMatrixAccessor matrixAccessor)
    {
        _originalImage = matrixAccessor;
        _separatorPointers.Add(new Pointer(0, 0, _originalImage.Width / 2, _originalImage.Height / 2));
        _separatorPointers.Add(new Pointer(_originalImage.Width / 2, 0, _originalImage.Width, _originalImage.Height / 2));
        _separatorPointers.Add(new Pointer(0, _originalImage.Height / 2, _originalImage.Width / 2, _originalImage.Height));
        _separatorPointers.Add(new Pointer(_originalImage.Width / 2, _originalImage.Height / 2 - 1, _originalImage.Width, _originalImage.Height));
    }

    public Bitmap Execute()
    {
        var resultBitmap = new Bitmap(_originalImage.Width, _originalImage.Height);
        for (int y = 0; y < _originalImage.Height; y++)
        {
            for (int x = 0; x < _originalImage.Width; x += 4)
            {
                for (int p = 0; p < _separatorPointers.Count; p++)
                {
                    var color = _originalImage.GetColorData(x + p, y);
                    var pixel = Color.FromArgb(color.A, color.R, color.G, color.B);
                    var position = _separatorPointers[p].GetCurrPosition();
                    resultBitmap.SetPixel(position.Item2, position.Item1, pixel);
                }
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
        _x = startX - 1;
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