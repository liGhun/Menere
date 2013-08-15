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
                    
                    byte[] bytes = System.Convert.FromBase64String(icon_base64);
                    if (bytes.Length > 32)
                    {
                        icon_path = AppController.app_data_path + "\\icons\\fever_" + this.id + ".ico";
                        System.IO.File.WriteAllBytes(icon_path, bytes);
                    }
                    else
                    {
                        icon_path = string.Format("https://plus.google.com/_/favicon?domain={0}&alt=feed", System.Web.HttpUtility.UrlEncode(fever_feed.site_url));
                    }
                }
                catch { }
            }
            if (string.IsNullOrEmpty(icon_path))
            {
                icon_path = string.Format("https://plus.google.com/_/favicon?domain={0}&alt=feed", System.Web.HttpUtility.UrlEncode(fever_feed.site_url));
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
            FeverAccount account = this.receiving_account as FeverAccount;
            if (account != null)
            {
                try
                {
                    IEnumerable<IItem> unread_items = account.unread_items.Where(item => item.feed_id == this.id);
                    if (unread_items != null)
                    {
                        List<FeverItem> items = new List<FeverItem>();
                        foreach (IItem item in unread_items)
                        {
                            FeverItem fever_item = item as FeverItem;
                            if (fever_item != null)
                            {
                                items.Add(fever_item);
                            }
                        }

                       
                        account.fever_account.mark_feed_as_read(this.fever_feed);
                        
                        foreach (FeverItem item in items)
                        {
                            item.is_read = true;
                            account.unread_items.Remove(item);
                        }
                        items = null;
                    }
                }
                catch { }
            }
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
