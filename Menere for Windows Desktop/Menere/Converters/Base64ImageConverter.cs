using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.IO;
using System.Windows.Media.Imaging;

namespace Menere.Converters
{
    public class Base64ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                string s = value as string;

                if (string.IsNullOrEmpty(s))
                    return null;

                BitmapImage bi = new BitmapImage();

                bi.BeginInit();
                byte[] bytes = System.Convert.FromBase64String(s);
                bi.StreamSource = new MemoryStream(bytes);
                bi.EndInit();

                return bi;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

    }
}
