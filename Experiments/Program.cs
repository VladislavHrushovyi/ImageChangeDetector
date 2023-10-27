// See https://aka.ms/new-console-template for more information

using System.Drawing;
using Experiments;

var image = (Bitmap)Image.FromFile("./images/1.png");
var imageBytes = image.ReadAsBytesStream();
Console.WriteLine(string.Join("-",imageBytes.Take(4)));
Console.Read();