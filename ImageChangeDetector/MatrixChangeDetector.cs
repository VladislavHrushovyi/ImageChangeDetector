using System.Drawing;

namespace ImageChangeDetector;

public class MatrixChangeDetector : IChangeDetector
{
    private readonly IEqualityComparer<ColorData> _comparer;

    public MatrixChangeDetector(IEqualityComparer<ColorData> comparer)
    {
        _comparer = comparer;
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
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!visited[y,x] && !_comparer.Equals(img1.GetColorData(x,y), img2.GetColorData(x,y)))
                {
                    Rectangle rectangle = GrowRectangle(img1, img2, visited, x, y);
                    rectangles.Add(rectangle);
                }
            }
        }
        return rectangles;
    }

    private Rectangle GrowRectangle(IMatrixAccessor img1, IMatrixAccessor img2, bool[,] visited, int startX, int startY)
    {
        int width = img1.Width;
        int height = img1.Height;

        int endX = startX;
        int endY = startY;

        while (endX + 1 < width 
               && !visited[startY, endX + 1] 
               && !_comparer.Equals(img1.GetColorData(endX + 1, startY), img2.GetColorData(endX + 1, startY))
               )
        {
            endX++;
            visited[startY, endX] = true;
        }

        while (endY + 1 < height)
        {
            bool canExpand = true;
            for (int x = startX; x <= endX; x++)
            {
                if (visited[endY + 1, x] 
                    || _comparer.Equals(img1.GetColorData(x, endY + 1), img2.GetColorData(x, endY + 1))
                    )
                {
                    canExpand = false;
                    break;
                }

                visited[endY + 1, x] = true;
            }

            if (canExpand)
            {
                endY++;
            }
            else
            {
                break;
            }
        }

        return new Rectangle(startX, startY, endY - startY + 1, endX - startX + 1);
    }
}

public record Rectangle(int Left, int Top, int Height, int Width);