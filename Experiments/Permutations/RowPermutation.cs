using System.Drawing;
using ImageChangeDetector;

namespace Experiments.Permutations;

public class RowPermutation : ITransformImage
{
    private readonly IMatrixAccessor _matrixAccessor;
    private readonly List<RowPointer> _rowPointers = new ();

    public RowPermutation(IMatrixAccessor matrixAccessor)
    {
        _matrixAccessor = matrixAccessor;
        _rowPointers.Add(new RowPointer(0, _matrixAccessor.Height / 2));
        _rowPointers.Add(new RowPointer(_matrixAccessor.Height / 2, _matrixAccessor.Height));
    }

    public Bitmap Execute()
    {
        var resultImage = new Bitmap(_matrixAccessor.Width, _matrixAccessor.Height);
        for (int y = 0; y < _matrixAccessor.Height - 1; y += 2)
        {
            for (int i = y; i < y + _rowPointers.Count; i++)
            {
                var row = _matrixAccessor.GetRangeByX(i, _matrixAccessor.Width).ToArray();
                var position = _rowPointers[i - y].GetCurrentRowPosition();
                
                WriteRow(resultImage, row, position);
            }
        }
        return resultImage;
    }

    private void WriteRow(Bitmap image, int[] values, int rowNumber)
    {
        for (int i = 0; i < values.Length; i++)
        {
            var colorData = ColorData.FromInt(values[i]);
            var color = Color.FromArgb(colorData.A, colorData.R, colorData.G, colorData.B);
            image.SetPixel(i, rowNumber, color);
        }
    }
    
}

class RowPointer
{
    private readonly int _endY;
    private int _y;

    public RowPointer(int startY, int endY)
    {
        _y = startY - 1;
        _endY = endY;
    }

    public int GetCurrentRowPosition()
    {
        _y++;
        if (_y == _endY)
        {
            return _endY - 1;
        }

        return _y;
    }
}