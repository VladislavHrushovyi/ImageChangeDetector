using System.Drawing;
using Experiments.Permutations;
using ImageChangeDetector;

namespace Experiments;

public class PermutationApplication
{
    private Bitmap Image;

    public PermutationApplication(Bitmap image)
    {
        Image = image;
    }

    public PermutationApplication ColumnPermutation()
    {
        var colPermutation = new ColumnPermutation(new MatrixAccessor(Image.AsMatrix()));
        Image = colPermutation.Execute();
        return new PermutationApplication(Image);
    }

    public PermutationApplication RowPermutation()
    {
        var rowPermutation = new RowPermutation(new MatrixAccessor(Image.AsMatrix()));
        Image = rowPermutation.Execute();

        return new PermutationApplication(Image);
    }

    public void SaveImage(string path)
    {
        Image.Save(path);
    }
}