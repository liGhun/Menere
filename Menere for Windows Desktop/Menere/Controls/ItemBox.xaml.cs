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
            AppController.active_external_services.CollectionChanged += active_external_services_CollectionChanged;
            this.DataContextChanged += ItemBox_DataContextChanged;
        }

        void ItemBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            update_context_menu();
        }

        private void update_context_menu()
        {
            ItemContextMenu item_context_menu = new ItemContextMenu();
            border_around_item.ContextMenu = item_context_menu.get_context_menu(this.DataContext as IItem);
        }

        private void active_external_services_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            update_context_menu();
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
                        AppController.Current.main_window.webbroser_navigate_to_url(item.url);
                       
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
