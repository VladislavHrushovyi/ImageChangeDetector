namespace ImageChangeDetector;

public class ColorEqualityComparer : IEqualityComparer<ColorData>
{
    private readonly double _tolerance;
    private readonly double _maxDifference = Math.Sqrt(255 * 255 * 4);

    public ColorEqualityComparer(double tolerance)
    {
        if (_tolerance < 0 || _tolerance > 1)
        {
            throw new ArgumentException("Tolerance must be between 0.0 and 1.0");
        }
        _tolerance = tolerance;
    }
    public bool Equals(ColorData x, ColorData y)
    {
        var diffA = Math.Abs(x.A - y.A);
        var diffR = Math.Abs(x.R - y.R);
        var diffG = Math.Abs(x.G - y.G);
        var diffB = Math.Abs(x.B - y.B);

        var totalDifference = Math.Sqrt(diffA * diffA + diffR * diffR + diffG * diffG + diffB * diffB);
        return totalDifference <= _maxDifference * _tolerance;
    }

    public int GetHashCode(ColorData obj)
    {
        return obj.GetHashCode();
    }
}