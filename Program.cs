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
            ReadAndUploadImage();
            //GetSubFolders();
        }


        static void ReadAndUploadImage()
        {
            FileStream fileStream = new FileStream("Receiver_Marantz_SR8015/Receiver_Marantz_SR8015.jpg", FileMode.Open);
            using (StreamReader reader = new StreamReader(fileStream))
            {
                using var image = Image.Load(fileStream);
                image.Mutate(x => x.Resize(400, 400));
                image.Save("uploads/Receiver_Marantz_SR8015-400x400.jpg");
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

        static Receiver_PageData GetPageData_Receiver(string jsonFilePath)
        {
            var r_pd = new Receiver_PageData();
            try
            {
                var json = File.ReadAllText(jsonFilePath);
                var jsonObj = JObject.Parse(json);
                r_pd.Name = jsonObj["name"].ToString();
                r_pd.Description = jsonObj["description"].ToString();
                r_pd.Price = jsonObj["price"].ToString();
                r_pd.Channels = jsonObj["channels"].ToString();
                r_pd.Power = jsonObj["power"].ToString();
                r_pd.Connections = jsonObj["connections"].ToString();
                r_pd.Video = jsonObj["video"].ToString();
                r_pd.Audio = jsonObj["audio"].ToString();
                r_pd.Room = jsonObj["room"].ToString();
                r_pd.Amazon = jsonObj["amazon"].ToString();
                var tips = jsonObj["tips"] as JArray;
                var tips_stringArray = tips.Select(t => t["tip"].ToString()).ToArray();
                r_pd.Tips = tips_stringArray;
            }
            catch (Exception ex)
            {

            }
            return r_pd;
        }

        static void GetSubFolders()
        {
            DirectoryInfo directory = new DirectoryInfo("./");
            DirectoryInfo[] directories = directory.GetDirectories();
            foreach (DirectoryInfo folder in directories)
            {
                var folderSplitArray = folder.Name.Split("_");
                if(folderSplitArray.Count() >= 3)
                {
                    if (folderSplitArray[1] == "How" && folderSplitArray[2] == "To")
                    {
                        var x = folder.Name;
                        var jsonFilePath = x + "/" + x + ".json";
                        var htc_pd = GetPageData_HowToChoose(jsonFilePath);
                        var templateFilePath = x + "/" + x + "_Template.txt";
                        var contentHtml = GenerateContentHtmlFromTemplate_HTC(templateFilePath, htc_pd);
                        var sql = GenerateSql_AddPage_Htc(contentHtml, htc_pd);

                    }
                    else if(folderSplitArray[0] == "Receiver")
                    {
                        var x = folder.Name;
                        var jsonFilePath = x + "/" + x + ".json";
                        var r_pd = GetPageData_Receiver(jsonFilePath);
                    }
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



        static string GenerateSql_AddPage_Htc(string contentHtml, HowToChoose_PageData htc_pd)
        {
            var pageId = DateTime.Now.ToString("yyyyMMddHHmmss");
            var pageUrl = string.Join("-", htc_pd.Title.Split(new char[] { '-', ' ' })).ToLower();

            var sql = "INSERT INTO wp_posts ( ID," +
                " post_author," +
                " post_date," +
                " post_date_gmt," +
                " post_content," +
                " post_title," +
                " post_excerpt," +
                " post_status," +
                " comment_status," +
                " ping_status," +
                " post_password," +
                " post_name," +
                " to_ping," +
                " pinged," +
                " post_modified," +
                " post_modified_gmt," +
                " post_content_filtered," +
                " post_parent," +
                " guid," +
                " menu_order," +
                " post_type," +
                " post_mime_type," +
                " comment_count ) VALUES (" +
                "" +
                pageId +
                "," +//ID-->POI
                " 1," + //post_author
                " now()," +//post_date
                " now()," +//post_date_gmt
                " '" +
                contentHtml +
                "'," +//post_content-->POI
                " '" +
                htc_pd.Title +
                "'," +//post_title-->POI
                " ''," +//post_excerpt
                " 'publish'," +//post_status
                " 'closed'," +//comment_status
                " 'closed'," +//ping_status
                " ''," +//post_password
                " '" +
                pageUrl +
                "'," +//post_name-->POI
                " ''," +//to_ping
                " ''," +//pinged
                " now()," +//post_modified
                " now()," +//post_modified_gmt
                " ''," +//post_content_filtered
                " 0," +//post_parent
                " ''," +//guid
                " 0," +//menu_order
                " 'page'," +//post_type
                " ''," +//post_mime_type
                " 0 )";//comment_count

            return sql;
        }

    }
}
