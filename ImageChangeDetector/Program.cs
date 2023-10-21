// See https://aka.ms/new-console-template for more information

using ImageChangeDetector;

var changerApplication = new ChangeDetectorApplication();

var result = changerApplication.Execute("./images/2/image1.png", "./images/2/image2.png");
result.Save("./images/1/image3.png");


