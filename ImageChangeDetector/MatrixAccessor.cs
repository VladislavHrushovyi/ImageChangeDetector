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

    public int Width { get; set; }
    public int Height { get; set; }

    public ColorData GetColorData(int x, int y)
        => ColorData.FromInt(_matrix[x, y]);
}