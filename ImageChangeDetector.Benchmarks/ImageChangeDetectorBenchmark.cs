using System.Drawing;
using BenchmarkDotNet.Attributes;

namespace ImageChangeDetector.Benchmarks;

[MemoryDiagnoser]
public class ImageChangeDetectorBenchmark
{
    private readonly IMatrixAccessor _img1;
    private readonly IMatrixAccessor _img2;

    private readonly IEqualityComparer<ColorData> _comparer = new ColorEqualityComparer(0.1d);
    private readonly MatrixChangeDetector _detector;
    private readonly MatrixChangeDetectorModified _detectorModified;
    private readonly MatrixChangeDetectorModifiedWithPrepare _detectorWithPrepare;

    public ImageChangeDetectorBenchmark()
    {
        _detector = new MatrixChangeDetector(_comparer);
        _detectorModified = new MatrixChangeDetectorModified(_comparer);
        _detectorWithPrepare = new MatrixChangeDetectorModifiedWithPrepare(_comparer);

        _img1 = new MatrixAccessor(((Bitmap)Image.FromFile("D:\\Project\\ImageChangeDetector\\ImageChangeDetector\\bin\\Release\\net7.0\\images\\4\\image1.png")).AsMatrix());
        _img2 = new MatrixAccessor(((Bitmap)Image.FromFile("D:\\Project\\ImageChangeDetector\\ImageChangeDetector\\bin\\Release\\net7.0\\images\\4\\image2.png")).AsMatrix());
    }

    [Benchmark(Baseline = true)]
    public List<Rectangle> ChangeDetectorOnDefaultImplementation()
    {
       return _detector.Detect(_img1, _img2);
    }
    [Benchmark]
    public List<Rectangle> ChangeOnModifiedImplementation()
    {
        return _detectorModified.Detect(_img1, _img2);
    }
    // public List<Rectangle> ChangeDetectorOnModifiedPrepared()
    // {
    //     return _detectorWithPrepare.Detect(_img1, _img2);
    // }
}