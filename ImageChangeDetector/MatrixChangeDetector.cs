using System.Drawing;

namespace ImageChangeDetector;

public class MatrixChangeDetector : IChangeDetector
{
    public List<Rectangle> Detect(IMatrixAccessor img1, IMatrixAccessor img2)
    {
        throw new NotImplementedException();
        return new();
    }
}

public record Rectangle(int Left, int Top, int Height, int Width);