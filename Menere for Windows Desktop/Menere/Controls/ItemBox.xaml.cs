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
    /// Interaction logic for ItemBox.xaml
    /// </summary>
    public partial class ItemBox : UserControl
    {
        public ItemBox()
        {
            InitializeComponent();
        }

        private void button_mark_read_Click(object sender, RoutedEventArgs e)
        {
             Button button = sender as Button;
             if (button != null)
             {
                 IItem item = button.DataContext as IItem;
                 if (item != null)
                 {
                     if (item.mark_read())
                     {
                         item.receiving_account.items.Remove(item);
                     }
                 }
             }
        }

        private void button_open_url_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                IItem item = button.DataContext as IItem;
                if (item != null)
                {
                    System.Diagnostics.Process.Start(item.url);
                }
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e != null)
            {
                if (e.ClickCount == 2)
                {
                    IItem item = this.DataContext as IItem;
                    if (item != null)
                    {
                        AppController.Current.main_window.webbrowser.Navigate(item.url);
                    }
                }
            }
        }

        private void image_faviconItem_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            try
            {
                IItem item = this.DataContext as IItem;
                if (item != null)
                {

                    BitmapImage image = new BitmapImage(new Uri(string.Format("https://plus.google.com/_/favicon?domain={0}&alt=feed", System.Web.HttpUtility.UrlEncode(item.feed.site_url)), UriKind.Relative));
                    image_faviconItem.Source = image;
                }
                else
                {
                    BitmapImage image = new BitmapImage(new Uri("/Menere;component/Images/MenereIcon.ico", UriKind.Relative));
                    image_faviconItem.Source = image;
                }
            }
            catch
            {
                BitmapImage image = new BitmapImage(new Uri("/Menere;component/Images/MenereIcon.ico", UriKind.Relative));
                image_faviconItem.Source = image;
            }
        }

  
    }
}
