using System.Drawing;

namespace ImageChangeDetector;

public class MatrixChangeDetector : IChangeDetector
{
    public int[,] Detect(Bitmap img1, Bitmap img2)
    {
        var matrix1 = img1.AsMatrix();
        var matrix2 = img2.AsMatrix();
        return Detect(matrix1, matrix2);
    }
    public int[,] Detect(int[,] matrix1, int[,] matrix2)
    {
        throw new NotImplementedException();
    }
}