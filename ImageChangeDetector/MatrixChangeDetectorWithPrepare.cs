using System.Drawing;

namespace ImageChangeDetector;

public class MatrixChangeDetectorModifiedWithPrepare : IChangeDetector
{
    private readonly IEqualityComparer<ColorData> _comparer;

    public MatrixChangeDetectorModifiedWithPrepare(IEqualityComparer<ColorData> comparer)
    {
        _comparer = comparer;
    }

    public List<Rectangle> Detect(IMatrixAccessor img1, IMatrixAccessor img2)
    {
        int width = Math.Min(img1.Width, img2.Width);
        int height = Math.Min(img1.Height, img2.Width);
        int maxDifferenceRangePixels = 5;
        List<Rectangle> rectangles = new();

        if (img2.Height > img1.Height)
        {
            rectangles.Add(new Rectangle(0, img1.Height, img2.Height - img1.Height, img2.Width));
        }

        if (img2.Width > img1.Width)
        {
            rectangles.Add(new Rectangle(img1.Width, 0, img1.Height, img2.Width - img1.Width));
        }

        HashSet<(int Y, int X)> coordinates = new HashSet<(int Y, int X)>();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!_comparer.Equals(img1.GetColorData(x, y), img2.GetColorData(x, y)))
                {
                    coordinates.Add((y, x));
                }
            }
        }

        while (coordinates.Count != 0)
        {
            Console.WriteLine($"{coordinates.Count}");
            var startPoint = coordinates.ElementAt(0);
            var tempRect = new Rectangle(startPoint.Item2, startPoint.Item1, 0, 0);
            coordinates.Remove(startPoint);
            if (!coordinates.Any())
            {
                break;
            }
            Rectangle rectangle = GrowRectangle(coordinates,startPoint, tempRect);
            rectangles.Add(rectangle);
        }
        return rectangles;
    }

    private Rectangle GrowRectangle(HashSet<(int X, int Y)> coordinates, (int Y, int X) startPoint, Rectangle rectangle)
    {
        var queue = new Queue<(int Y, int X)>();
        queue.Enqueue(startPoint);
        coordinates.Remove(startPoint);

        while (queue.Count > 0)
        {
            var currentPoint = queue.Dequeue();
            var nearestPoints = coordinates
                .Where(p => CalcDistance(currentPoint, p) < 5)
                .ToArray();

            foreach (var nearestPoint in nearestPoints)
            {
                queue.Enqueue(nearestPoint);
                coordinates.Remove(nearestPoint);
                if (rectangle.Height < Math.Abs(rectangle.Top - startPoint.Y))
                {
                    rectangle = rectangle with { Height = (Math.Abs(rectangle.Top - startPoint.Y)) + 2 };
                }
                
                if (rectangle.Width < Math.Abs(rectangle.Left - startPoint.X))
                {
                    rectangle = rectangle with { Width = (Math.Abs(rectangle.Left - startPoint.X)) + 2 };
                }
                
                if (rectangle.Left > startPoint.X)
                {
                    rectangle = rectangle with
                    {
                        Left = startPoint.X,
                        Width = rectangle.Width + Math.Abs(startPoint.X - rectangle.Left)
                    };
                }
            }
        }

        return rectangle;
    }
    private double CalcDistance((int Y, int X) point1, (int Y, int X) point2)
    {
        var differenceX = point2.X - point1.X;
        var differenceY = point2.Y - point1.Y;
        var sum = differenceX * differenceX + differenceY * differenceY;
        return Math.Sqrt(sum);
    }
}