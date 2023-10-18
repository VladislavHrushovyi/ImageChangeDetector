namespace ImageChangeDetector;

public interface IMatrixAccessor
{
    int Width { get; }
    int Height { get; }

    ColorData GetColorData(int x, int y);
    IEnumerable<int> GetRangeByX(int y, int amount);
    IEnumerable<int> GetRangeByY(int x, int amount);
}