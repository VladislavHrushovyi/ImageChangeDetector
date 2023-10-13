namespace ImageChangeDetector;

public interface IMatrixAccessor
{
    int Width { get; set; }
    int Height { get; set; }

    ColorData GetColorData(int x, int y);
}