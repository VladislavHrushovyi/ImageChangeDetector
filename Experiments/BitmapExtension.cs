using System.Drawing;
using System.Drawing.Imaging;

namespace Experiments;

public static class BitmapExtension
{
    public static IEnumerable<int> ReadAsBytesStream(this Bitmap bitmap)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            bitmap.Save(stream, ImageFormat.Png);
            using (BinaryReader binaryReader = new BinaryReader(stream))
            {
                while (stream.Position < stream.Length)
                {
                    yield return binaryReader.Read();
                }
            }
        }
    }
}