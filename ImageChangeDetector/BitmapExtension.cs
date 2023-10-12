using System.Drawing;
using System.Drawing.Imaging;

namespace ImageChangeDetector;

public static class BitmapExtension
{
    public static int[,] AsMatrix(this Bitmap image)
    {
        int[,] array2D = new int[image.Width, image.Height];

#pragma warning disable CA1416
        BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format32bppRgb);
#pragma warning restore CA1416

        unsafe
        {
            byte* address = (byte*)bitmapData.Scan0;

            int paddingOffset = bitmapData.Stride - (image.Width * 4);//4 bytes per pixel

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    byte[] temp = new byte[4];
                    temp[0] = address[0];
                    temp[1] = address[1];
                    temp[2] = address[2];
                    temp[3] = address[3];
                    array2D[i, j] = BitConverter.ToInt32(temp, 0);
                    address += 4;
                }

                address += paddingOffset;
            }
        }
        image.UnlockBits(bitmapData);

        return array2D;
    }
}