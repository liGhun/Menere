﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using Menere.Model;

namespace Menere.Controls
{
    public class ItemContextMenu
    {


        public ContextMenu get_context_menu(IItem item)
        {
            try
            {
                ContextMenu context_menu = new ContextMenu();

                MenuItem menu_mark_read = new MenuItem();
                if (item == null)
                {
                    menu_mark_read.Header = "Toggle read state";
                    menu_mark_read.Icon = get_icon("button_mark_item_read");
                }
                else
                {
                    if (item.is_read)
                    {
                        menu_mark_read.Header = "Mark unread";
                        menu_mark_read.Icon = get_icon("button_mark_item_unread");
                    }
                    else
                    {
                        menu_mark_read.Header = "Mark read";
                        menu_mark_read.Icon = get_icon("button_mark_item_read");
                    }
                }
                menu_mark_read.DataContext = item;
                menu_mark_read.Click += menu_mark_read_Click;
                context_menu.Items.Add(menu_mark_read);

                MenuItem menu_mark_save = new MenuItem();
                if (item == null)
                {
                    menu_mark_save.Header = "Toggle saved state";
                    menu_mark_save.Icon = get_icon("button_save_item");
                }
                else
                {
                    if (item.is_saved)
                    {
                        menu_mark_save.Header = "Mark unsaved";
                        menu_mark_save.Icon = get_icon("button_saved_item");
                    }
                    else
                    {
                        menu_mark_save.Header = "Mark saved";
                        menu_mark_save.Icon = get_icon("button_save_item");
                    }
                }
                menu_mark_save.DataContext = item;
                menu_mark_save.Click += menu_mark_save_Click;
                context_menu.Items.Add(menu_mark_save);

                MenuItem menu_edit_item = new MenuItem();
                menu_edit_item.Header = "Edit item";
                menu_edit_item.DataContext = item;
                menu_edit_item.Icon = get_icon("image_tags");
                menu_edit_item.Click += menu_edit_item_Click;
                context_menu.Items.Add(menu_edit_item);

                MenuItem menu_send_to = new MenuItem();
                menu_send_to.Header = "Send to...";

                MenuItem menu_item_send_mail = new MenuItem();
                menu_item_send_mail.Header = "Email";
                // Icon
                menu_item_send_mail.DataContext = item;
                menu_item_send_mail.Click += menu_item_send_mail_Click;
                menu_send_to.Items.Add(menu_item_send_mail);

                if (AppController.active_external_services.Count > 0)
                {
                    foreach (ShareSharp.IShareService service in AppController.active_external_services)
                    {
                        MenuItem menu_item_service = new MenuItem();
                        menu_item_service.Header = service.Name;
                        // Icon
                        menu_item_service.DataContext = item;
                        menu_item_service.Click += menu_item_service_Click;
                        menu_item_service.CommandParameter = service;

                        menu_send_to.Items.Add(menu_item_service);
                    }
                }

                context_menu.Items.Add(menu_send_to);

                return context_menu;
            }
            catch
            {
                return null;
            }
        }

        void menu_mark_read_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu_item = sender as MenuItem;
            if (menu_item != null)
            {
                IItem item = menu_item.DataContext as IItem;

                if (item == null && Properties.Settings.Default.use_listView)
                {
                    item = AppController.Current.main_window.listview_items.listview_items.SelectedItem as IItem;
                }
                else
                {
                    item = AppController.Current.main_window.listbox_items.listview_items.SelectedItem as IItem;
                }

                if (item != null)
                {
                    if (item.is_read)
                    {
                        item.mark_unread();
                        if (!item.receiving_account.unread_items.Contains(item))
                        {
                            item.receiving_account.unread_items.Add(item);
                        }
                        menu_item.Header = "Mark read";
                        menu_item.Icon = get_icon("button_mark_item_read");
                    }
                    else
                    {
                        item.mark_read();
                        if (item.receiving_account.unread_items.Contains(item))
                        {
                            item.receiving_account.unread_items.Remove(item);
                        }
                        menu_item.Header = "Mark unread";
                        menu_item.Icon = get_icon("button_mark_item_unread");
                    }
                }
            }
        }

        void menu_mark_save_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu_item = sender as MenuItem;
            if (menu_item != null)
            {
                IItem item = menu_item.DataContext as IItem;

                if (item == null && Properties.Settings.Default.use_listView)
                {
                    item = AppController.Current.main_window.listview_items.listview_items.SelectedItem as IItem;
                }
                else
                {
                    item = AppController.Current.main_window.listbox_items.listview_items.SelectedItem as IItem;
                }

                if (item != null)
                {
                    if (item.is_saved)
                    {
                        item.mark_unsaved();
                        if (item.receiving_account.saved_items.Contains(item))
                        {
                            item.receiving_account.saved_items.Remove(item);
                        }
                        menu_item.Header = "Mark saved";
                        menu_item.Icon = get_icon("button_save_item");
                    }
                    else
                    {
                        item.mark_saved();
                        if (!item.receiving_account.saved_items.Contains(item))
                        {
                            item.receiving_account.saved_items.Add(item);
                        }
                        menu_item.Header = "Mark unsaved";
                        menu_item.Icon = get_icon("button_saved_item");
                    }
                }
            }
        }

        void menu_edit_item_Click(object sender, RoutedEventArgs e)
        {
             MenuItem menu_item = sender as MenuItem;
             if (menu_item != null)
             {
                 IItem item = menu_item.DataContext as IItem;

                 if (item == null && Properties.Settings.Default.use_listView)
                 {
                     item = AppController.Current.main_window.listview_items.listview_items.SelectedItem as IItem;
                 }

                 if (item != null)
                 {
                     UserInterface.EditItem edit_item = new UserInterface.EditItem();
                     edit_item.DataContext = item;
                     edit_item.Show();
                 }
             }
        }

        private System.Windows.Controls.Image get_icon(string icon_source, bool is_ressource_name = true)
        {
            if (!string.IsNullOrEmpty(icon_source))
            {
                System.Windows.Controls.Image icon = new System.Windows.Controls.Image();
                icon.Source = System.Windows.Application.Current.Resources[icon_source] as System.Windows.Media.ImageSource;

                return icon;
            }
            else
            {
                return null;
            }
        }

        void menu_item_send_mail_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu_item = sender as MenuItem;
            if (menu_item != null)
            {
                IItem item = menu_item.DataContext as IItem;

                if (item == null && Properties.Settings.Default.use_listView)
                {
                    item = AppController.Current.main_window.listview_items.listview_items.SelectedItem as IItem;
                }

                if (item != null)
                {
                    string body = Helper.Texts.get_command_line_text(item.html, max_length: 200);
                    body += "%0A%0A" + item.url;

                    System.Diagnostics.Process.Start(string.Format("mailto:{0}?subject={1}&body={2}", "", item.title, body));
                }
            }
        }

        void menu_item_service_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu_item = sender as MenuItem;
            if (menu_item != null)
            {
                IItem item = menu_item.DataContext as IItem;
                ShareSharp.IShareService service = menu_item.CommandParameter as ShareSharp.IShareService;

                if (item == null && Properties.Settings.Default.use_listView)
                {
                    item = AppController.Current.main_window.listview_items.listview_items.SelectedItem as IItem;
                }

                if (item != null && service != null)
                {
                    string text = item.html;
                    if (string.IsNullOrEmpty(text))
                    {
                        text = "";
                    }
                    text = Helper.Texts.remove_html(text);
                    service.SendNow(item.title, text, item.url);
                }
            }
        }
    }
}
