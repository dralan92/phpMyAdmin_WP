using Grpc.Core;
using Newtonsoft.Json.Linq;
using phpMyAdmin.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace phpMyAdmin
{
    class Program
    {
        static void Main(string[] args)
        {

            ResizeImage("Receiver_Marantz_SR8015/", "Receiver_Marantz_SR8015.jpg", 400, 400, 963, 444);


            //GetSubFolders();
        }


        //static void ReadAndUploadImage()
        //{
        //    FileStream fileStream = new FileStream("Receiver_Marantz_SR8015/Receiver_Marantz_SR8015.jpg", FileMode.Open);
        //    using (StreamReader reader = new StreamReader(fileStream))
        //    {
        //        using var image = Image.Load(fileStream);
        //        image.Mutate(x => x.Resize(400, 400));
        //        image.Save("uploads/Receiver_Marantz_SR8015-400x400.jpg");
        //    }
           
        //}
        //public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        //{
        //    var destRect = new System.Drawing.Rectangle(0, 0, width, height);
        //    var destImage = new Bitmap(width, height);

        //    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        //    using (var graphics = Graphics.FromImage(destImage))
        //    {
        //        graphics.CompositingMode = CompositingMode.SourceCopy;
        //        graphics.CompositingQuality = CompositingQuality.HighQuality;
        //        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //        graphics.SmoothingMode = SmoothingMode.HighQuality;
        //        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        //        using (var wrapMode = new ImageAttributes())
        //        {
        //            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
        //            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
        //        }
        //    }

        //    return destImage;
        //}

        private static void ResizeImage(string path, string originalFilename,
                     /* note changed names */
                     int canvasWidth, int canvasHeight,
                     /* new */
                     int originalWidth, int originalHeight)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(path + originalFilename);

            System.Drawing.Image thumbnail =
                new Bitmap(canvasWidth, canvasHeight); // changed parm names
            System.Drawing.Graphics graphic =
                         System.Drawing.Graphics.FromImage(thumbnail);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            /* ------------------ new code --------------- */

            // Figure out the ratio
            double ratioX = (double)canvasWidth / (double)originalWidth;
            double ratioY = (double)canvasHeight / (double)originalHeight;
            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            int posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
            int posY = Convert.ToInt32((canvasHeight - (originalHeight * ratio)) / 2);

            graphic.Clear(System.Drawing.Color.White); // white padding
            graphic.DrawImage(image, posX, posY, newWidth, newHeight);

            /* ------------- end new code ---------------- */

            System.Drawing.Imaging.ImageCodecInfo[] info =
                             ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters;
            encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality,
                             100L);
            thumbnail.Save("uploads/" + newWidth + "." + originalFilename, info[1],
                             encoderParameters);
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
