// See https://aka.ms/new-console-template for more information

using System.Drawing;
using Experiments;

var image = (Bitmap)Image.FromFile("./images/1.png");
var imageSeparator = new ImageSeparator(image);

var result = imageSeparator.Execute();

result.Save("./images/result.png");