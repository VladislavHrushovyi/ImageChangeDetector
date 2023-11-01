// See https://aka.ms/new-console-template for more information

using System.Drawing;
using Experiments;
using ImageChangeDetector;

var image = (Bitmap)Image.FromFile("./images/1.png");
var imageSeparator = new ColumnPermutation(new MatrixAccessor(image.AsMatrix()));

var result = imageSeparator.Execute();

result.Save("./images/result.png");