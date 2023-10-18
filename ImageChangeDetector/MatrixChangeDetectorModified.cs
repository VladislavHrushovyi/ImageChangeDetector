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
        int maxDistanceDifference = 2;
        
        int endX = startX;
        int endY = startY;
        ContainBadPixelByX(img1, img2, visited, ref endX, endY, maxDistanceDifference);
        ContainBadPixelByY(img1, img2, visited, endX, ref endY, maxDistanceDifference);

        return new Rectangle(startX, startY, endY - startY + 1, endX - startX + 1);
    }

    private bool ContainBadPixelByX(
        IMatrixAccessor img1, 
        IMatrixAccessor img2, 
        bool[,] visited, 
        ref int startX, int startY, 
        int maxDistanceDifference)
    {
        bool hasDifferenceByX = true;
        while (hasDifferenceByX)
        {
            var pixelsFromImg1 = img1.GetRangeByX(startY, maxDistanceDifference).ToArray();
            var pixelsFromImg2 = img2.GetRangeByX(startY, maxDistanceDifference).ToArray();

            var compareResults = Enumerable.Range(0, maxDistanceDifference - 1)
                .Select(i => _comparer.Equals(
                    ColorData.FromInt(pixelsFromImg1[i]), 
                    ColorData.FromInt(pixelsFromImg2[i])
                    )).Where(x => x);
            for (int x = startX + 1; x < startX + maxDistanceDifference && x < img1.Width; x++)
            {
                visited[startY, x] = true;
            }
            hasDifferenceByX = !compareResults.Any();
            startX += maxDistanceDifference;
        }
        return hasDifferenceByX;
    }
    private bool ContainBadPixelByY(
        IMatrixAccessor img1, 
        IMatrixAccessor img2, 
        bool[,] visited, 
        int startX, ref int startY, 
        int maxDistanceDifference)
    {
        bool hasDifferenceByY = true;
        while (hasDifferenceByY)
        {
            var pixelsFromImg1 = img1.GetRangeByY(startX, maxDistanceDifference).ToArray();
            var pixelsFromImg2 = img2.GetRangeByY(startX, maxDistanceDifference).ToArray();

            var compareResults = Enumerable.Range(0, maxDistanceDifference - 1)
                .Select(i => _comparer.Equals(
                    ColorData.FromInt(pixelsFromImg1[i]), 
                    ColorData.FromInt(pixelsFromImg2[i])
                )).Where(x => x);
            for (int y = startY + 1; y < startY + maxDistanceDifference && y < img1.Height; y++)
            {
                visited[y, startX] = true;
            }
            hasDifferenceByY = !compareResults.Any();
            startY += maxDistanceDifference;
        }
        return hasDifferenceByY;
    }
}