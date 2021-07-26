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
            
            GetSubFolders();
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

        static HowToChoose_PageData GetPageData_HowToChoose(string jsonFilePath)
        {
            var htc_pd = new HowToChoose_PageData();
            try
            {
                var json = File.ReadAllText(jsonFilePath);
                var jsonObj = JObject.Parse(json);
                htc_pd.Title = jsonObj["title"].ToString();
                htc_pd.Description = jsonObj["description"].ToString();
                var features = jsonObj["features"] as JArray;
                var features_stringArray = features.Select(f => f["feature"].ToString()).ToArray();
                htc_pd.Features = features_stringArray;

            }
            catch(Exception ex)
            {

            }
            return htc_pd;

        }

        static void GetSubFolders()
        {
            DirectoryInfo directory = new DirectoryInfo("./");
            DirectoryInfo[] directories = directory.GetDirectories();
            foreach (DirectoryInfo folder in directories)
            {
                var folderSplitArray = folder.Name.Split("_");
                if(folderSplitArray.Count() >=3)
                if (folderSplitArray[1] == "How" && folderSplitArray[2] == "To")
                {
                    var x = folder.Name;
                    var jsonFilePath = x + "/" + x + ".json";
                    var htc_pd = GetPageData_HowToChoose(jsonFilePath);
                    var templateFilePath = x + "/" + x + "_Template.txt";
                    var contentHtml = GenerateContentHtmlFromTemplate_HTC(templateFilePath, htc_pd);

                }
            }


        }

        static string GenerateContentHtmlFromTemplate_HTC(string templatePath, HowToChoose_PageData htc_pd)
        {
            string contentHtml = File.ReadAllText(templatePath);
            string listItem = "";
            for(int i=0; i< htc_pd.Features.Count(); i++)
            {
                listItem += "<li>" + htc_pd.Features[i] + "</li>";
            }
            string unOrderedList = "<ul>" + listItem + "</ul>";
            contentHtml = contentHtml
                .Replace("Receiver_How_To_Choose_Template_Description", "<p>" + htc_pd.Description + "</p>")
                .Replace("Receiver_How_To_Choose_Template_Features", unOrderedList);

            File.WriteAllText("ContentHtml.txt", contentHtml);
            return unOrderedList;
        }





    }
}
