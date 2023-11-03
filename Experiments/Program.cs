// See https://aka.ms/new-console-template for more information

using System.Drawing;
using Experiments;

var image = (Bitmap)Image.FromFile("./images/1.png");
var app = new PermutationApplication(image);
app.ColumnPermutation()
    .RowPermutation()
    .ColumnPermutation()
    .RowPermutation()
    .SaveImage("./images/result.png");