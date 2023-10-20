using System.Xml.Linq;

namespace ImageChangeDetector;

public class MatrixChangeDetectorModified : IChangeDetector
{
    private readonly IEqualityComparer<ColorData> _comparer;

    public MatrixChangeDetectorModified(ColorEqualityComparer comparer)
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

        HashSet<Tuple<int, int>> coordinates = new HashSet<Tuple<int, int>>();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!_comparer.Equals(img1.GetColorData(x,y),img2.GetColorData(x,y)))
                {
                    coordinates.Add(new Tuple<int, int>(y, x));
                }
            }
        }

        while (coordinates.Count != 0)
        {
            var tempRect = new Rectangle(coordinates.ElementAt(0).Item2, coordinates.ElementAt(0).Item1, 0, 0);
            coordinates.Remove(coordinates.ElementAt(0));
            if (!coordinates.Any())
            {
                break;
            }
            Rectangle rectangle = GrowRectangle(coordinates,coordinates.ElementAt(0), tempRect);
            rectangles.Add(rectangle);
        }
        
        return rectangles;
    }

    private Rectangle GrowRectangle(HashSet<Tuple<int, int>> coordinates,Tuple<int, int> startPoint,  Rectangle rectangle)
    {
        var nearestPoints = coordinates.Where(x => CalcDistance(startPoint, x) < 5).ToList();
        if (!nearestPoints.Any())
        {
            if (rectangle.Height < Math.Abs(rectangle.Top - startPoint.Item1))
            {
                rectangle = rectangle with { Height = Math.Abs(rectangle.Top - startPoint.Item1) };
            }
        
            if (rectangle.Width < Math.Abs(rectangle.Left - startPoint.Item2))
            {
                rectangle = rectangle with { Width = Math.Abs(rectangle.Left - startPoint.Item2) };
            }
            return rectangle;
        }
        foreach (var nearestPoint in nearestPoints)
        {
            coordinates.Remove(nearestPoint);
            var tempRect = GrowRectangle(coordinates, nearestPoint, rectangle);
            if (rectangle.Height < tempRect.Height)
            {
                rectangle = rectangle with { Height = tempRect.Height };
            }
            if (rectangle.Width < tempRect.Width)
            {
                rectangle = rectangle with { Width = tempRect.Width };
            }
        }
        return rectangle;
    }

    private double CalcDistance(Tuple<int, int> point1, Tuple<int, int> point2)
    {
        var differenceX = point2.Item2 - point1.Item2;
        var differenceY = point2.Item1 - point1.Item1;
        var sum = differenceX * differenceX + differenceY * differenceY;
        return Math.Sqrt(sum);
    }
}