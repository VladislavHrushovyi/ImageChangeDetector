using System.Linq;

namespace ImageChangeDetector;

public class MatrixAccessor : IMatrixAccessor
{
    private readonly int[,] _matrix;

    public MatrixAccessor(int[,] matrix)
    {
        this._matrix = matrix;
        Width = matrix.GetLength(1);
        Height = matrix.GetLength(0);
    }
    
    public int Width { get; }
    public int Height { get; }

    public ColorData GetColorData(int x, int y)
        => ColorData.FromInt(_matrix[y, x]);

    public IEnumerable<int> GetRangeByX(int y, int amount)
    {
        return Enumerable.Range(0, amount).Select(i => _matrix[y, i]);
    }

    public IEnumerable<int> GetRangeByY(int x, int amount)
    {
        return Enumerable.Range(0, amount).Select(i => _matrix[i, x]);
    }
}