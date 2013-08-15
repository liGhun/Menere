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
using System.Threading;
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

            if (AppController.active_external_services.Count > 0)
            {
                MenuItem menu_send_to = new MenuItem();
                menu_send_to.Header = "Send to...";

                foreach (ShareSharp.IShareService service in AppController.active_external_services)
                {

                    MenuItem menu_item_service = new MenuItem();
                    menu_item_service.Header = service.Name;
                    menu_item_service.PreviewMouseDown += menu_item_service_PreviewMouseDown;
                    menu_item_service.Click += menu_item_service_Click;
                    menu_item_service.CommandParameter = service;
                    menu_send_to.Items.Add(service);
                }

                border_around_item.ContextMenu.Items.Add(menu_send_to);
            }
        }

        void menu_item_service_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MenuItem menu_item = sender as MenuItem;
            if (menu_item != null)
            {
                IItem item = this.DataContext as IItem;
                ShareSharp.IShareService service = menu_item.CommandParameter as ShareSharp.IShareService;
                if (item != null && service != null)
                {
                    string text = item.html;
                    if (string.IsNullOrEmpty(text))
                    {
                        text = "";
                    }
                    text = System.Text.RegularExpressions.Regex.Replace(text, "<.*?>", string.Empty);
                    service.SendNow(item.title, text, item.url);
                }
            }
        }

        void menu_item_service_Click(object sender, RoutedEventArgs e)
        {

            MenuItem menu_item = sender as MenuItem;
            if (menu_item != null)
            {
                IItem item = this.DataContext as IItem;
                ShareSharp.IShareService service = menu_item.CommandParameter as ShareSharp.IShareService;
                if (item != null && service != null)
                {
                    string text = item.html;
                    if (string.IsNullOrEmpty(text))
                    {
                        text = "";
                    }
                    text = System.Text.RegularExpressions.Regex.Replace(text, "<.*?>", string.Empty);
                    service.SendNow(item.title, text, item.url);
                }
            }
        }

        private void button_mark_read_Click(object sender, RoutedEventArgs e)
        {
             Button button = sender as Button;
             if (button != null)
             {
                 IItem item = button.DataContext as IItem;
                 if (item != null)
                 {
                    if (!item.is_read)
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                            {
                                item.mark_read();
                            });
                            item.is_read = true;
                            try {
                                item.receiving_account.unread_items.Remove(item);
                            } catch {}
                        }
                        else
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                             {
                                item.mark_unread();
                             });
                             item.is_read = false;
                             try
                             {
                                 item.receiving_account.unread_items.Add(item);
                             }
                             catch { }
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

        private void button_save_item_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                IItem item = button.DataContext as IItem;
                if (item != null)
                {
                    if (!item.is_saved)
                    {
                        ThreadPool.QueueUserWorkItem(delegate
                         {
                             item.mark_saved();
                         });
                        try
                        {
                            item.receiving_account.saved_items.Add(item);
                        }
                        catch { }
                    }
                    else
                    {
                        ThreadPool.QueueUserWorkItem(delegate
                         {
                             item.mark_unsaved();
                         });
                        try
                        {
                            item.receiving_account.saved_items.Remove(item);
                        }
                        catch { }
                    }

                }
            }
        }

        private void contextMenu_edit_tags_Click(object sender, RoutedEventArgs e)
        {
            UserInterface.EditItem edit_item = new UserInterface.EditItem();
            edit_item.DataContext = this.DataContext;
            edit_item.ShowInTaskbar = true;
            edit_item.Show();
        }

  
    }
}
