using System.Drawing;

namespace ImageChangeDetector;

public class MatrixChangeDetector : IChangeDetector
{

    private readonly IEqualityComparer<ColorData> _comparer;

    public MatrixChangeDetector()
    {
        _comparer = new ColorEqualityComparer(0.1);
    }

    public List<Rectangle> Detect(IMatrixAccessor img1, IMatrixAccessor img2)
    {
        int width = Math.Min(img1.Width, img2.Width);
        int height = Math.Min(img1.Height, img2.Width);

        List<Rectangle> rectangles = new();

        if (img2.Height > img1.Height)
        {
            rectangles.Add(new Rectangle(0, img1.Height, img2.Height - img1.Height, img2.Width));
        }
        if (img2.Width > img1.Width)
        {
            rectangles.Add(new Rectangle(img1.Width, 0, img2.Height, img2.Width - img1.Width)); 
        }

        bool[,] visited = new bool[height, width];
        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                if (!visited[x,y] && _comparer.Equals(img1.GetColorData(x,y), img2.GetColorData(x,y)))
                {
                    Rectangle rectangle = GrowRectangle(img1, img2, visited, x, y);
                    rectangles.Add(rectangle);
                }
            }
        }
        return new();
    }

    private Rectangle GrowRectangle(IMatrixAccessor img1, IMatrixAccessor img2, bool[,] visited, int startX, int startY)
    {
        throw new NotImplementedException();
    }
}

public record Rectangle(int Left, int Top, int Height, int Width);