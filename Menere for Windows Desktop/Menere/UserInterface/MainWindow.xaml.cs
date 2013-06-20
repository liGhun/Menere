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
using System.Reflection;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Menere.Model;

namespace Menere.UserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Model.IAccount account;
        
        public DispatcherTimer update_timer;
        public string current_filter_string { get; set; }
        public Model.IFeed current_filter_feed { get; set; }
        public Model.IFolder current_filter_folder { get; set; }
        private int last_selected_index { get; set; }
        public ObservableCollection<IItem> current_shown_items { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();

            button_show_saved.Visibility = System.Windows.Visibility.Collapsed;

            combobox_accounts.Visibility = System.Windows.Visibility.Collapsed;
            button_remove_feed_filter.Visibility = System.Windows.Visibility.Collapsed;
            button_remove_folder_filter.Visibility = System.Windows.Visibility.Collapsed;

            account = AppController.Current.current_account as Model.IAccount;
            listbox_feeds.ItemsSource = account.feeds;
            listbox_groups.ItemsSource = account.groups;
            listbox_items.Items.SortDescriptions.Add(new SortDescription("created", ListSortDirection.Ascending));
            listbox_feeds.Items.SortDescriptions.Add(new SortDescription("title", ListSortDirection.Ascending));
            listbox_groups.Items.SortDescriptions.Add(new SortDescription("name", ListSortDirection.Ascending));
            listbox_items.ItemsSource = account.items;
            current_shown_items = account.items;
            current_shown_items.CollectionChanged += unread_items_CollectionChanged;
            webbrowser.Navigated += webbrowser_Navigated;
            textblock_item_title.Text = "";

           // filter_feeds();

            update_timer = new DispatcherTimer();
            update_timer.Tick +=update_timer_Tick;
            update_timer.Interval = new TimeSpan(0, 3, 0);
            update_timer.Start();
        }

        void update_timer_Tick(object sender, EventArgs e)
        {
 	        button_refresh_Click(null,null);
        }

        void unread_items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            update_header_items();
            
            if (AppController.Current.current_account.initial_fetch_completed)
            {
                filter_feeds();
                filter_items();
            }
        }
        private void update_header_items()
        {
            button_show_unread.Content = string.Format("Unread items ({0})", account.unread_items.Count());
            button_show_all.Content = string.Format("All items ({0})", account.items.Count());
            button_show_saved.Content = string.Format("Saved items ({0})", account.saved_items.Count());
        }
        



        private void listbox_items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Model.IItem item = listbox_items.SelectedItem as Model.IItem;
            if (item == null && listbox_items.Items.Count > 0)
            {
                listbox_items.SelectedIndex = Math.Min(last_selected_index,Math.Max(0, listbox_items.Items.Count - 1));
                return;
            }

            if (listbox_items.SelectedItem != null)
            {
                last_selected_index = Math.Max(0, listbox_items.SelectedIndex);
            }

            if (item != null)
            {
                textblock_item_title.Text = item.title;
                textblock_feed_title.Text = item.feed.title;
                if (string.IsNullOrWhiteSpace(item.html) || item.title.ToLower().Trim() == item.html.ToLower().Trim())
                {
                    webbrowser.Navigate(item.url);
                }
                else
                {
                    webbrowser.NavigateToString(create_html_content(item));
                }
            }
            else
            {
                textblock_feed_title.Text = "";
                textblock_item_title.Text = "";
                webbrowser.NavigateToString("&nbsp;");
            }
        }

        public void HideScriptErrors(WebBrowser wb, bool Hide)
        {
            FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (fiComWebBrowser == null) return;

            object objComWebBrowser = fiComWebBrowser.GetValue(wb);

            if (objComWebBrowser == null) return;

            objComWebBrowser.GetType().InvokeMember(

            "Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { Hide });

        }


        void webbrowser_Navigated(object sender, NavigationEventArgs e)
        {
            HideScriptErrors(webbrowser,
            true);
        }

        public  void button_refresh_Click(object sender, RoutedEventArgs e)
        {
            foreach (IAccount account_available in AppController.accounts)
            {
                account_available.update_all_feeds();
            }
        }

        private string create_html_content(Model.IItem item)
        {
            SolidColorBrush background = System.Windows.Application.Current.Resources["color_content_background"] as SolidColorBrush;
            SolidColorBrush text_color = System.Windows.Application.Current.Resources["color_main_foreground"] as SolidColorBrush;
            SolidColorBrush link_color = System.Windows.Application.Current.Resources["color_link"] as SolidColorBrush;
            string back_string = "#" + background.ToString().Substring(3);
            string text_string = "#" + text_color.ToString().Substring(3);;
            string link_string = "#" + link_color.ToString().Substring(3);;
            string html_content = string.Format(
                "<html>\n " +
                " <head>\n" +
                "  <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/>\n" +
                "  <title>{0}</title>\n" +
                "  <style type=\"text/css\">\n" +
                "   <!--\n" +
                "    html {{ background: {1}; color:{2} }}\n" +
                "    a {{ color:{3} }}\n" +
                "   -->\n" +
                "  </style>\n" +
                " </head>\n" +
                " <body>\n" +
                " <div id=\"content\">\n" +
                "  {4}\n" +
                " </div>\n" +
                " <div id=\"link_to_full_article\">\n" +
                "  <a href=\"{5}\">Read full article</a>\n" +
                " </div>\n" +
                "</html>",
                item.title, back_string, text_string, link_string, item.html, item.url);
            return html_content;
        }

        private void listbox_items_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e!= null) 
            {
                if (e.Key == Key.Space)
                {
                    Model.IItem item = listbox_items.SelectedItem as Model.IItem;
                    if (item != null)
                    {
                        if (!item.is_read)
                        {
                            if (item.mark_read())
                            {
                                item.receiving_account.unread_items.Remove(item);
                                item.is_read = true;
                            }
                        }
                        else
                        {
                            if (item.mark_read())
                            {
                                item.receiving_account.unread_items.Add(item);
                                item.is_read = false;
                            }
                        }
                    }
                }

                if (e.Key == Key.J)
                {
                    move_within_items(1);
                }
                if (e.Key == Key.K)
                {
                    move_within_items(-1);
                }
                if (e.Key == Key.Return)
                {
                    Model.IItem item = listbox_items.SelectedItem as Model.IItem;
                    if (item != null)
                    {
                        webbrowser.Navigate(item.url);
                    }
                }
                if (e.Key == Key.Right)
                {
                    Model.IItem item = listbox_items.SelectedItem as Model.IItem;
                    if (item != null)
                    {
                        System.Diagnostics.Process.Start(item.url);
                    }
                    e.Handled = true;
                }
            }
        }

        public void move_within_items(int number_of_items)
        {
            if (listbox_items.Items.Count > 0)
            {
                listbox_items.SelectedIndex = Math.Max(0,Math.Min(listbox_items.Items.Count - 1, listbox_items.SelectedIndex + number_of_items));
            }
        }

        private void listbox_feeds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            current_filter_feed = listbox_feeds.SelectedItem as Model.IFeed;
            if (current_filter_feed == null)
            {
                button_remove_feed_filter.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                button_remove_feed_filter.Visibility = System.Windows.Visibility.Visible;
            }

            filter_items();
            update_header_items();
        }

        private void listbox_groups_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            current_filter_folder = listbox_groups.SelectedItem as Model.IFolder;
            if (current_filter_folder == null)
            {
                button_remove_folder_filter.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                button_remove_folder_filter.Visibility = System.Windows.Visibility.Visible;
            }
            listbox_feeds.SelectedItem = null;
            update_all_filter();
            update_header_items();
        }

        public void update_all_filter()
        {
            filter_feeds();
            filter_items();
        }

        private void filter_feeds()
        {
            listbox_feeds.Items.Filter = delegate(object obj)
               {
                   Model.IFeed feed = obj as Model.IFeed;
                   if (feed == null)
                   {
                       return false;
                   }
                   try
                   {
                       IEnumerable<Model.IItem> items_in_this_feed = current_shown_items.Where(item => item.feed_id == feed.id);
                       if (items_in_this_feed == null)
                       {
                           return false;
                       }
                       else if (items_in_this_feed.Count() == 0)
                       {
                           return false;
                       }
                       if (current_filter_folder != null)
                       {
                           if (current_filter_folder.feeds.Contains(feed))
                           {
                               return true;
                           }
                           else
                           {
                               return false;
                           }
                       }
                       else
                       {
                           return true;
                       }
                   }
                   catch
                   {
                       return true;
                   }
               };
        }

        private void filter_items()
        {
            listbox_items.Items.Filter = delegate(object obj)
               {
                   Model.IItem item = obj as Model.IItem;

                   if (item == null)
                   {
                       return false;
                   }

                   bool feed_filter = true;
                   bool string_filter = true;
                   bool folder_filter = true;
                   if (current_filter_feed != null)
                   {
                       if (current_filter_feed.id != item.feed_id)
                       {
                           feed_filter = false;
                       }
                   }
                   if (current_filter_folder != null)
                   {
                       if (!current_filter_folder.feeds.Contains(item.feed))
                       {
                           folder_filter = false;
                       }
                   }

                   if (!string.IsNullOrWhiteSpace(current_filter_string))
                   {
                       string_filter = (item.title.ToLower().Contains(current_filter_string.ToLower()) || item.html.ToLower().Contains(current_filter_string.ToLower()));
                   }

                   return (feed_filter && string_filter && folder_filter);
               };
        }

        private void button_remove_feed_filter_Click(object sender, RoutedEventArgs e)
        {
            this.listbox_feeds.SelectedItem = null;
        }

        private void textbox_filter_text_TextChanged(object sender, TextChangedEventArgs e)
        {
            current_filter_string = textbox_filter_text.Text;
            filter_items();
        }

        private void button_add_account_Click(object sender, RoutedEventArgs e)
        {
            UserInterface.Add_Account add_account = new Add_Account();
            add_account.Show();
        }

        private void button_remove_folder_filter_Click(object sender, RoutedEventArgs e)
        {
            ClearTreeViewItemsControlSelection(listbox_groups.Items, listbox_groups.ItemContainerGenerator);
        }

        private static void ClearTreeViewItemsControlSelection(ItemCollection ic, ItemContainerGenerator icg)
        {
            if ((ic != null) && (icg != null))
            {
                for (int i = 0; i < ic.Count; i++)
                {
                    TreeViewItem tvi = icg.ContainerFromIndex(i) as TreeViewItem;
                    if (tvi != null)
                    {
                        ClearTreeViewItemsControlSelection(tvi.Items, tvi.ItemContainerGenerator);
                        tvi.IsSelected = false;
                    }
                }
            }
        }

        private void combobox_accounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IAccount chosen_account = combobox_accounts.SelectedItem as IAccount;
            if (chosen_account != null)
            {
                AppController.Current.current_account = chosen_account;
                account = chosen_account;
                if (account.GetType() == typeof(FeverAccount) || account.GetType() == typeof(FeedlyAccount))
                {
                    button_show_saved.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    button_show_saved.Visibility = System.Windows.Visibility.Collapsed;
                }
                this.textblock_foldername.Text = account.folder_name;
                listbox_feeds.ItemsSource = account.feeds;
                listbox_groups.ItemsSource = account.groups;
                button_show_all_Click(null, null);
                update_header_items();
            }
        }

        public void button_show_unread_Click(object sender, RoutedEventArgs e)
        {
            grid_main.Focus();
            button_show_unread.Background = System.Windows.Application.Current.Resources["color_active_items_list"] as SolidColorBrush;
            button_show_unread.IsEnabled = false;

            button_show_all.Background = System.Windows.Application.Current.Resources["color_content_background"] as SolidColorBrush;
            button_show_all.IsEnabled = true;

            button_show_saved.Background = System.Windows.Application.Current.Resources["color_content_background"] as SolidColorBrush;
            button_show_saved.IsEnabled = true;

            current_shown_items.CollectionChanged -= unread_items_CollectionChanged;
            listbox_items.ItemsSource = account.unread_items;
            current_shown_items = account.unread_items;
            current_shown_items.CollectionChanged += unread_items_CollectionChanged;
            unread_items_CollectionChanged(null, null);
            filter_feeds();
            listbox_items.UpdateLayout();
        }

        public void button_show_saved_Click(object sender, RoutedEventArgs e)
        {
            grid_main.Focus();
            button_show_unread.Background = System.Windows.Application.Current.Resources["color_content_background"] as SolidColorBrush;
            button_show_unread.IsEnabled = true;

            button_show_all.Background = System.Windows.Application.Current.Resources["color_content_background"] as SolidColorBrush;
            button_show_all.IsEnabled = true;

            button_show_saved.Background = System.Windows.Application.Current.Resources["color_active_items_list"] as SolidColorBrush;
            button_show_saved.IsEnabled = false;

            current_shown_items.CollectionChanged -= unread_items_CollectionChanged;
            listbox_items.ItemsSource = account.saved_items;
            current_shown_items = account.saved_items;
            current_shown_items.CollectionChanged += unread_items_CollectionChanged;
            unread_items_CollectionChanged(null, null);
            filter_feeds();
            listbox_items.UpdateLayout();
        }

        public void button_show_all_Click(object sender, RoutedEventArgs e)
        {
            grid_main.Focus();
            button_show_unread.Background = System.Windows.Application.Current.Resources["color_content_background"] as SolidColorBrush;
            button_show_unread.IsEnabled = true;

            button_show_all.Background = System.Windows.Application.Current.Resources["color_active_items_list"] as SolidColorBrush;
            button_show_all.IsEnabled = false;

            button_show_saved.Background = System.Windows.Application.Current.Resources["color_content_background"] as SolidColorBrush;
            button_show_saved.IsEnabled = true;

            current_shown_items.CollectionChanged -= unread_items_CollectionChanged;
            listbox_items.ItemsSource = account.items;
            current_shown_items = account.items;
            current_shown_items.CollectionChanged += unread_items_CollectionChanged;
            unread_items_CollectionChanged(null, null);
            filter_feeds();
            listbox_items.UpdateLayout();
        }

        
    }
}
