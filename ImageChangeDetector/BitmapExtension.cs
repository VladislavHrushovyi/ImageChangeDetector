using System.Drawing;
using System.Drawing.Imaging;

namespace ImageChangeDetector;

public static class BitmapExtension
{
    public static int[,] AsMatrix(this Bitmap image)
    {
        var height = image.Height;
        var width = image.Width;
        int[,] array2D = new int[height, width];

#pragma warning disable CA1416
        BitmapData bitmapData = image.LockBits(new System.Drawing.Rectangle(0, 0, width, height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppRgb);
#pragma warning restore CA1416
        try
        {
            int stride = bitmapData.Stride;
            IntPtr scan0 = bitmapData.Scan0;
            
            unsafe
            {
                byte* address = (byte*)(void*)scan0;

                //int paddingOffset = bitmapData.Stride - (image.Width * 4);//4 bytes per pixel

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int blue = address[0];
                        int green = address[1];
                        int red = address[2];
                        int alpha = address[3];

                        int pixelValue = ((alpha << 24) | (red << 16) | (green << 8) | blue);

                        array2D[y, x] = pixelValue;

                        address += 4;
                    }

                    address += stride - (width * 4);
                }
            }
        }
        finally
        {
            image.UnlockBits(bitmapData);   
        }

        return array2D;
    }
}