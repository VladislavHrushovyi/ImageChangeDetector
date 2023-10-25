// See https://aka.ms/new-console-template for more information

using ImageChangeDetector;

var changerApplication = new ChangeDetectorApplication();

var result = changerApplication.Execute("./images/3/image1.png", "./images/3/image2.png");
result.Save("./images/3/image3.png");


