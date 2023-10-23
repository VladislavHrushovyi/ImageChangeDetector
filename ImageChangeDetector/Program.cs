// See https://aka.ms/new-console-template for more information

using ImageChangeDetector;

var changerApplication = new ChangeDetectorApplication();

var result = changerApplication.Execute("./images/4/image1.png", "./images/4/image2.png");
result.Save("./images/4/image3.png");


