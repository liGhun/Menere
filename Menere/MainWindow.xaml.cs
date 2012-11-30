using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms.Integration;
using WebKit;
using ReaderSharp.Model;


namespace Menere
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private System.Windows.Forms.Integration.WindowsFormsHost _MyHost;
        public WebKit.WebKitBrowser webkitBrowser;

        public MainWindow()
        {
            InitializeComponent();

      /*      _MyHost = new System.Windows.Forms.Integration.WindowsFormsHost();
            webkitBrowser = new WebKit.WebKitBrowser();
            _MyHost.Child = webkitBrowser;
            WebkitBrowserGrid.Children.Add(_MyHost); */

            listBoxFeeds.ItemsSource = AppController.Current.AllFeedsWithUnreadItems;
        }

        private void listBoxFeeds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          
        }

        private void listBoxItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listbox = sender as ListBox;
            if (listbox != null)
            {
                if (listbox.SelectedItem != null)
                {
                    Item item = listbox.SelectedItem as Item;
                    if (item != null)
                    {
                        webkitBrowser.Url = new Uri(item.FullVersionUrl);
                    }
                }
            }
        }

        private void buttonUpdateAll_Click(object sender, RoutedEventArgs e)
        {
            AppController.Current.UpdateAll();
        }

        private void buttonChangeFeedList_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBoxFeeds.ItemsSource == AppController.Current.AllFeedsWithUnreadItems)
            {
                buttonChangeFeedList.Content = "All";
                listBoxFeeds.ItemsSource = AppController.Current.AllFeeds;
            }
            else
            {
                buttonChangeFeedList.Content = "Unread";
                listBoxFeeds.ItemsSource = AppController.Current.AllFeedsWithUnreadItems;
            }
        }
    }
}
