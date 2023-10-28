using System.Drawing;
using System.Drawing.Imaging;

namespace Experiments;

public static class BitmapExtension
{
    public static IEnumerable<Color> AsStreamPixel(this Bitmap bitmap)
    {
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                yield return bitmap.GetPixel(x, y);
            }
        }
    }
}