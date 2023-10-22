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

        bool[,] visited = new bool[height, width];
        //List<(int Y, int X)> coordinates = new List<(int Y, int X)>();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!visited[y, x] && !_comparer.Equals(img1.GetColorData(x, y), img2.GetColorData(x, y)))
                {
                    Rectangle rectangle = new Rectangle(x, y, 0, 0);
                    rectangle = GrowRectangle(img1, img2, (Y: y, X: x), rectangle, visited);
                    rectangles.Add(rectangle);
                    Console.WriteLine($"ADD RECTANGLE {rectangles.Count}");
                }
            }
        }

        // while (coordinates.Count != 0)
        // {
        //     var startPoint = coordinates.ElementAt(0);
        //     var tempRect = new Rectangle(startPoint.Item2, startPoint.Item1, 0, 0);
        //     coordinates.Remove(startPoint);
        //     if (!coordinates.Any())
        //     {
        //         break;
        //     }
        //     Rectangle rectangle = GrowRectangle(coordinates,startPoint, tempRect);
        //     rectangles.Add(rectangle);
        // }

        return rectangles;
    }

    private Rectangle GrowRectangle(List<(int X, int Y)> coordinates, (int Y, int X) startPoint, Rectangle rectangle)
    {
        var nearestPoints = coordinates.Where(x => CalcDistance(startPoint, x) < 5).ToArray();
        if (!nearestPoints.Any())
        {
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
                if (rectangle.Left > tempRect.Left)
                {
                    rectangle = rectangle with
                    {
                        Left = tempRect.Left,
                    };
                }
            }
        }

        return rectangle;
    }

    private Rectangle GrowRectangle(IMatrixAccessor img1, IMatrixAccessor img2, (int Y, int X) startPoint,
        Rectangle rectangle, bool[,] visited)
    {
        var points = GetPixelsByAxisX(img1, img2, 1, 4, startPoint, visited)
            .Concat(GetPixelsByAxisX(img1, img2, -1, 4, startPoint, visited))
            .Concat(GetPixelsByAxisY(img1, img2, 1, 4, startPoint, visited))
            .Concat(GetPixelsByAxisY(img1, img2, -1, 4, startPoint, visited)).ToArray();
        if (points.Any())
        {
            var maxX = points.Max(x => x.X);
            var maxY = points.Max(y => y.Y);
            var minX = points.Min(p => p.X);
            if (rectangle.Height < Math.Abs(rectangle.Top - maxY))
            {
                rectangle = rectangle with { Height = (Math.Abs(rectangle.Top - maxY)) + 2 };
            }

            if (rectangle.Width < Math.Abs(rectangle.Left - maxX))
            {
                rectangle = rectangle with { Width = (Math.Abs(rectangle.Left - maxX)) + 2 };
            }

            if (rectangle.Left > minX)
            {
                rectangle = rectangle with
                {
                    Left = minX,
                    Width = rectangle.Width + Math.Abs(minX - rectangle.Left)
                };
            }
            
            foreach (var point in points)
            {
                var tempRect = GrowRectangle(img1, img2, point, rectangle, visited);
                if (rectangle.Height < tempRect.Height)
                {
                    rectangle = rectangle with { Height = tempRect.Height };
                }

                if (rectangle.Width < tempRect.Width)
                {
                    rectangle = rectangle with { Width = tempRect.Width };
                    if (rectangle.Left > tempRect.Left)
                    {
                        rectangle = rectangle with
                        {
                            Left = tempRect.Left,
                        };
                    }
                }
            }

            return rectangle;
        }
        return rectangle;
    }

    private IEnumerable<(int Y, int X)> GetPixelsByAxisX(IMatrixAccessor img1, IMatrixAccessor img2, int direction, int amount,
        (int Y, int X) startPoint, bool[,] visited)
    {
        int startX = startPoint.X;
        int endX = startX + direction * amount;
        while (startX != endX)
        {
            if (startX == endX)
            {
                yield break;
            }
            if (startX > img1.Width - 1 || startX < 0)
            {
                yield break;
            }

            if (!visited[startPoint.Y, startX] && !_comparer.Equals(img1.GetColorData(startX, startPoint.Y), img2.GetColorData(startX, startPoint.Y)))
            {
                yield return (startPoint.Y, startX);
            }

            visited[startPoint.Y, startX] = true;
            startX += direction;
        }
    }

    private IEnumerable<(int Y, int X)> GetPixelsByAxisY(IMatrixAccessor img1,IMatrixAccessor img2, int direction, int amount,
        (int Y, int X) startPoint, bool[,] visited)
    {
        int startY = startPoint.Y;
        int endY = startY + direction * amount;
        while (startY != endY)
        {
            if (startY > img1.Height - 1 || startY < 0)
            {
                yield break;
            }

            if (!visited[startY, startPoint.X] && !_comparer.Equals(img1.GetColorData(startPoint.X, startY), img2.GetColorData(startPoint.X, startY)))
            {
                yield return (startY, startPoint.X);   
            }

            visited[startY, startPoint.X] = true;
            startY += direction;
        }
    }

    private double CalcDistance((int Y, int X) point1, (int Y, int X) point2)
    {
        var differenceX = point2.X - point1.X;
        var differenceY = point2.Y - point1.Y;
        var sum = differenceX * differenceX + differenceY * differenceY;
        return Math.Sqrt(sum);
    }
}