using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;

namespace phpMyAdmin
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ReadAndUploadImage();
        }


        static void ReadAndUploadImage()
        {
            FileStream fileStream = new FileStream("image.jpg", FileMode.Open);
            using (StreamReader reader = new StreamReader(fileStream))
            {
                using var image = Image.Load(fileStream);
                image.Mutate(x => x.Resize(150, 150));
                image.Save("image-150X150.jpg");
            }
           
        }
    }
}
