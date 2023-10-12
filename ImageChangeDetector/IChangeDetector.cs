namespace ImageChangeDetector;

public interface IChangeDetector
{
    public int[,] Detect(int[,] matrix1, int[,] matrix2);
}