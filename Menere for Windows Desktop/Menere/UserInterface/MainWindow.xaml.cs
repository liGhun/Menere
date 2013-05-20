﻿using System;
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
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Menere.UserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Model.FeverAccount account;
        
        public DispatcherTimer update_timer;
        public string current_filter_string { get; set; }
        public Model.IFeed current_filter_feed { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();

            account = AppController.accounts[0] as Model.FeverAccount;
            listbox_feeds.ItemsSource = account.feeds;

            
            listbox_items.ItemsSource = account.items;
            account.items.CollectionChanged += unread_items_CollectionChanged;
            webbrowser.Navigated += webbrowser_Navigated;
            textblock_item_title.Text = "";
            Button_Click(null, null);

            filter_feeds();

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
            textblock_header_unread_items.Text = string.Format("Unread items ({0})", account.items.Count());
            filter_feeds();
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
           // SharpFever.Model.FeverResponse feeds = account.get_feeds();
           // listbox_feeds.ItemsSource = feeds.feeds;

      //      List<uint> ids = new List<uint>();
      //      ids.Add(25041);
      //      SharpFever.Model.FeverResponse items = account.get_items(with_ids:ids);
      //      listbox_items.ItemsSource = items.items;

           //  SharpFever.Model.FeverResponse groups = account.get_groups();
           //  listbox_groups.ItemsSource = groups.groups;
           //  SharpFever.Model.FeverResponse favicons = account.get_favicons();
           // System.Drawing.Image image = favicons.favicons.First().image;

            button_refresh_Click(null, null);

        }

        private void listbox_items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Model.IItem item = listbox_items.SelectedItem as Model.IItem;
            if (item == null && listbox_items.Items.Count > 0)
            {
                listbox_items.SelectedIndex = 0;
                return;
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

        private void button_refresh_Click(object sender, RoutedEventArgs e)
        {
            account.update_all_feeds();
        }

        private string create_html_content(Model.IItem item)
        {
            SolidColorBrush background = System.Windows.Application.Current.Resources["color_main_background"] as SolidColorBrush;
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
                        if (item.mark_read())
                        {
                            item.receiving_account.items.Remove(item);
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
                       IEnumerable<Model.IItem> items_in_this_feed = account.items.Where(item => item.feed_id == feed.id);
                       if (items_in_this_feed == null)
                       {
                           return false;
                       }
                       else if (items_in_this_feed.Count() == 0)
                       {
                           return false;
                       }
                       return true;
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
                   if (current_filter_feed != null)
                   {
                       if (current_filter_feed.id != item.feed_id)
                       {
                           feed_filter = false;
                       }
                   }

                   if (!string.IsNullOrWhiteSpace(current_filter_string))
                   {
                       string_filter = (item.title.ToLower().Contains(current_filter_string.ToLower()) || item.html.ToLower().Contains(current_filter_string.ToLower()));
                   }

                   return (feed_filter && string_filter);
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
        
    }
}