using Newtonsoft.Json.Linq;
using phpMyAdmin.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;

namespace phpMyAdmin
{
    class Program
    {
        static void Main(string[] args)
        {
            var htcr_pageData = GetPageData_HowToChooseReceiver();
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

        static HowToChooseReceiver_PageData GetPageData_HowToChooseReceiver()
        {
            try
            {
                var json = File.ReadAllText("Receiver_How_To_Choose.json");
                var jsonObj = JObject.Parse(json);//.ToObject<HowToChooseReceiver_PageData>();
                var features = jsonObj["features"] as JArray;
                var features_stringArray = features.Select(f => f["feature"].ToString()).ToArray();

            }
            catch(Exception ex)
            {

            }
            return new HowToChooseReceiver_PageData();

        }


    }
}
