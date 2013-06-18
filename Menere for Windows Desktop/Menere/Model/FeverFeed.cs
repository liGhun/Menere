using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SharpFever.Model;

namespace Menere.Model
{
    public class FeverFeed : IFeed
    {
        public SharpFever.Model.Feed fever_feed { get; set; }
        public IAccount receiving_account { get; set; }
        public ObservableCollection<IItem> items
        {
            get
            {
                return receiving_account.items.Where(item => item.feed_id == this.id) as ObservableCollection<IItem>;
            }
            set
            {

            }
        }

        public FeverFeed(SharpFever.Model.Feed feed, FeverAccount account)
        {
            items = new ObservableCollection<IItem>();
            fever_feed = feed;
            receiving_account = account;
            
            if(!string.IsNullOrEmpty(icon_base64)) {
                try
                {
                    icon_path = AppController.app_data_path + "\\icons\\fever_" + this.id + ".ico";
                    byte[] bytes = System.Convert.FromBase64String(icon_base64);
                    System.IO.File.WriteAllBytes(icon_path, bytes);
                }
                catch { }
            }
            if (string.IsNullOrEmpty(icon_path))
            {
                icon_path = "/Menere;component/Images/MenereIcon.ico";
            }
        }

        ~FeverFeed() {
            if (!string.IsNullOrEmpty(icon_path))
            {
                if (System.IO.File.Exists(icon_path))
                {
                    try
                    {
                        System.IO.File.Delete(icon_path);
                    }
                    catch { }
                }
            }
        }

        public override string ToString()
        {
            return title;
        }

        public string id
        {
            get
            {
                return fever_feed.id.ToString();
            }
            set
            {

                fever_feed.id = Convert.ToUInt32(value);
            }
        }

        public string title
        {
            get
            {
                return fever_feed.title;
            }
            set
            {
                fever_feed.title = value;
            }
        }

        public string url
        {
            get
            {
                return fever_feed.url;
            }
            set
            {
                fever_feed.url = value;
            }
        }

        public string site_url
        {
            get
            {
                return fever_feed.site_url;
            }
            set
            {
                fever_feed.site_url = value;
            }
        }

        public void update_items()
        {
            return;
        }

        public void mark_all_items_read()
        {
        /*    SharpFever.Model.FeverResponse unread_items_ids = receiving_account.get_unread_item_ids();
            if (unread_items_ids != null)
            {
                if (unread_items_ids.unread_item_ids_list != null && !string.IsNullOrEmpty(unread_items_ids.unread_item_ids))
                {
                    SharpFever.Model.FeverResponse unread_fever_items = receiving_account.get_items(with_ids: unread_items_ids.unread_item_ids_list);
                    foreach (SharpFever.Model.Item fever_item in unread_fever_items.items)
                    {
                        Model.FeverItem item = new Model.FeverItem(fever_item);
                        item.receiving_account = account;
                        unread_items.Add(new Model.FeverItem(fever_item));
                    }
                    listbox_items.ItemsSource = unread_items;
                }
            } */
        }





        public string icon_base64
        {
            get
            {
                FeverAccount fever_account = this.receiving_account as FeverAccount;
                try
                {
                    SharpFever.Model.FavIcon favicon = fever_account.favicons.Where(fav => fav.id == fever_feed.favicon_id).First();
                    return favicon.base64;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                // never ever
            }
        }


        public string icon_path
        {
            get;
            set;
        }
    }
}
