// See https://aka.ms/new-console-template for more information

using System.Drawing;
using Experiments;
using ImageChangeDetector;

var image = (Bitmap)Image.FromFile("./images/1.png");
var pixels = image.AsStreamPixel();
