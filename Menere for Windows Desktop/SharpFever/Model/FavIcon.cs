using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;

namespace SharpFever.Model
{
    public class FavIcon
    {
        public uint id { get; set; }
        public string data { get; set; }
        public Image image
        {
            get
            {
                if (string.IsNullOrEmpty(data))
                {
                    return null;
                }
                else
                {
                    try
                    {
                        return Base64ToImage(parse_image_data(data));
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }

        public string type
        {
            get
            {
                return Regex.Replace(data, ";.*$", "");
            }
        }
        public string base64
        {
            get
            {
                return parse_image_data(data);
            }
        }

        private string parse_image_data(string image_data) {
            string parsed_data = image_data;
            parsed_data = Regex.Replace(image_data,"^.*?;","");
            parsed_data = parsed_data.Replace("base64,", "");

            return parsed_data;
        }

        private string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        private Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }
    }
}