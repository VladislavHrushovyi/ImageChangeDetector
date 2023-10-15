using System.Drawing;

namespace ImageChangeDetector;

public class ChangeDetectorApplication
{
    public Bitmap Execute(string pathImage1, string pathImage2)
    {
        var bitmap1 = (Bitmap)Image.FromFile(pathImage1);
        var bitmap2 = (Bitmap)Image.FromFile(pathImage2);
        return Execute(bitmap1, bitmap2);
    }
    public Bitmap Execute(Bitmap bitmap1, Bitmap bitmap2)
    {
        var detector = new MatrixChangeDetector(new ColorEqualityComparer(0.1));
        var rectangles = detector.Detect(new MatrixAccessor(bitmap1.AsMatrix()), new MatrixAccessor(bitmap2.AsMatrix()));

        return DrawRectangles(bitmap2, rectangles);
    }

    private Bitmap DrawRectangles(Bitmap bitmap, List<Rectangle> rectangles)
    {
        var bmp = new Bitmap(bitmap);
        using Graphics gfx = Graphics.FromImage(bmp);
        using Pen pen = new Pen(Color.Red, 1);
        foreach (var rect in rectangles)
        {
            gfx.DrawRectangle(pen, rect.Left, rect.Top, rect.Width, rect.Height);
        }

        return bmp;
    }
}