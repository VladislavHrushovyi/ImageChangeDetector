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
        
        HashSet<int> X = new HashSet<int>();
        HashSet<int> Y = new HashSet<int>();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!_comparer.Equals(img1.GetColorData(x,y), img2.GetColorData(x,y)))
                {
                    X.Add(x);
                    Y.Add(y);
                }
            }
        }

        var left = X.ElementAt(0);
        var top = Y.ElementAt(0);
        int heightY = 0;
        int widthX = 0;
        int positionX = 0;
        for (int y = 0; y < Y.Count - 1; y += 2)
        {
            if (Math.Abs(Y.ElementAt(y) - Y.ElementAt(y + 1)) > maxDifferenceRangePixels)
            {
                heightY = Math.Abs(Y.ElementAt(y) - top);
                y += 1;
                for (int x = positionX; x < X.Count - 1; x += 2)
                {
                    if (Math.Abs(X.ElementAt(x) - X.ElementAt(x + 1)) > maxDifferenceRangePixels)
                    {
                        widthX = Math.Abs(X.ElementAt(x) - left);
                        positionX = x + 2;
                        rectangles.Add(new Rectangle(left - 2, top - 2, heightY + 2, widthX + 2));
                        left = X.ElementAt(positionX);
                        top = Y.ElementAt(y);
                        break;
                    }
                }
            }
        }
        return rectangles;
    }
}