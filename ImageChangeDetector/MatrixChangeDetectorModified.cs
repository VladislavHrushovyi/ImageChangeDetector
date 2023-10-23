﻿namespace ImageChangeDetector;

public class MatrixChangeDetectorModified : IChangeDetector
{
    private readonly IEqualityComparer<ColorData> _comparer;

    public MatrixChangeDetectorModified(IEqualityComparer<ColorData> comparer)
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
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!visited[y, x] && !_comparer.Equals(img1.GetColorData(x, y), img2.GetColorData(x, y)))
                {
                    Rectangle rectangle = new Rectangle(x, y, 0, 0);
                    rectangle = GrowRectangle(img1, img2, (Y: y, X: x), rectangle, visited);
                    rectangles.Add(rectangle);
                }
            }
        }

        return rectangles;
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
}