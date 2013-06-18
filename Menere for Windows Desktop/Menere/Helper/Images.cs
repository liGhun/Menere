using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Menere.Helper
{
    public class Images
    {
        public static System.Windows.Controls.Image WFormsImageToWPFImage(System.Drawing.Image Old_School_Image)
        {
            MemoryStream ms = new MemoryStream();
            Old_School_Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            System.Windows.Media.Imaging.BitmapImage bImg = new System.Windows.Media.Imaging.BitmapImage();
            bImg.BeginInit();
            bImg.StreamSource = new MemoryStream(ms.ToArray());
            bImg.EndInit();

            System.Windows.Controls.Image WPFImage = new System.Windows.Controls.Image();
            WPFImage.Source = bImg;
            return WPFImage;
        }
    }
}
