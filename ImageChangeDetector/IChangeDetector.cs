namespace ImageChangeDetector;

public interface IChangeDetector
{
    public List<Rectangle> Detect(IMatrixAccessor matrix1, IMatrixAccessor matrix2);
}