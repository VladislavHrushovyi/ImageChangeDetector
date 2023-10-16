namespace ImageChangeDetector;

public interface IMatrixAccessor
{
    int Width { get; }
    int Height { get; }

    ColorData GetColorData(int x, int y);
}