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
                 FeverItem item = button.DataContext as FeverItem;
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
                FeverItem item = button.DataContext as FeverItem;
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
                    FeverItem item = this.DataContext as FeverItem;
                    if (item != null)
                    {
                        AppController.Current.main_window.webbrowser.Navigate(item.url);
                    }
                }
            }
        }
    }
}
