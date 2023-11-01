using System.Drawing;
using ImageChangeDetector;

namespace Experiments;

public class ColumnPermutation : ITransformImage
{
    private readonly IMatrixAccessor _matrixAccessor;
    private readonly List<ColumnPointer> _columnPointers = new();

    public ColumnPermutation(IMatrixAccessor matrixAccessor)
    {
        _matrixAccessor = matrixAccessor;
        _columnPointers.Add(new ColumnPointer(0, matrixAccessor.Width / 2));
        _columnPointers.Add(new ColumnPointer(matrixAccessor.Width / 2, matrixAccessor.Width));
    }

    public Bitmap Execute()
    {
        var resultImage = new Bitmap(_matrixAccessor.Width, _matrixAccessor.Height);
        for (int x = 0; x < _matrixAccessor.Width - 1; x+=2)
        {
            var column1 = _matrixAccessor.GetRangeByY(x, _matrixAccessor.Height).ToArray();
            var column2 = _matrixAccessor.GetRangeByY(x + 1, _matrixAccessor.Height).ToArray();

            var pointer1 = _columnPointers[0].GetCurrentColumn();
            var pointer2 = _columnPointers[1].GetCurrentColumn();
            
            WriteColumn(resultImage, column1, pointer1);
            WriteColumn(resultImage, column2, pointer2);
        }

        return resultImage;
    }

    private void WriteColumn(Bitmap image, int[] values, int numberColumn)
    {
        for (int y = 0; y < image.Height; y++)
        {
            var colorData = ColorData.FromInt(values[y]);
            var color = Color.FromArgb(colorData.A, colorData.R, colorData.G, colorData.B);
            image.SetPixel(numberColumn, y, color);
        }
    }
}

class ColumnPointer
{
    private readonly int _startX;
    private readonly int _endX;
    private int _x;

    public ColumnPointer(int startX, int width)
    {
        _startX = startX;
        _endX = width;
        _x = startX - 1;
    }

    public int GetCurrentColumn()
    {
        _x++;
        if (_x == _endX)
        {
            return _endX - 1;
        }

        return _x;
    }
}