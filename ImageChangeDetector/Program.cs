// See https://aka.ms/new-console-template for more information

using ImageChangeDetector;

var changerApplication = new ChangeDetectorApplication();

var result = changerApplication.Execute("./images/image1.png", "./images/image2.png");
result.Save("./images/image3.png");


