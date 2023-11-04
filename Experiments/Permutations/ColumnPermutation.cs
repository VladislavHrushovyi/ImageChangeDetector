using System.Drawing;
using ImageChangeDetector;

namespace Experiments.Permutations;

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
            for (int i = x; i < x + _columnPointers.Count; i++)
            {
                var column = _matrixAccessor.GetRangeByY(i, _matrixAccessor.Height).ToArray();
                var pointer = _columnPointers[i - x].GetCurrentColumn();
                WriteColumn(resultImage, column, pointer);
            }
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
    private readonly int _endX;
    private int _x;

    public ColumnPointer(int startX, int width)
    {
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