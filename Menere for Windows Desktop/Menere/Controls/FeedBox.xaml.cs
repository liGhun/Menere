using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Menere.Model;

namespace Menere.Controls
{
    /// <summary>
    /// Interaction logic for FeedBox.xaml
    /// </summary>
    public partial class FeedBox : UserControl
    {
        public FeedBox()
        {
            InitializeComponent();
        }

        private void image_favicon_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            try
            {
                IFeed feed = this.DataContext as IFeed;
                if (feed != null)
                {

                    BitmapImage image = new BitmapImage(new Uri(string.Format("https://plus.google.com/_/favicon?domain={0}&alt=feed", System.Web.HttpUtility.UrlEncode(feed.site_url)), UriKind.Relative));
                    image_favicon.Source = image;
                }
                else
                {
                    BitmapImage image = new BitmapImage(new Uri("/Menere;component/Images/MenereIcon.ico", UriKind.Relative));
                    image_favicon.Source = image;
                }
            }
            catch
            {
                BitmapImage image = new BitmapImage(new Uri("/Menere;component/Images/MenereIcon.ico", UriKind.Relative));
                image_favicon.Source = image;
            }
        }    }
}
