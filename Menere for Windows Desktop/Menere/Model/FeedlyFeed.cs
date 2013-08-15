using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSSharp.Feedly.Model;

namespace Menere.Model
{
    class FeedlyFeed : IFeed
    {
        public FeedlyFeed(IAccount account, Feed feed)
        {
            feedly_feed = feed;
            receiving_account = account;
        }

        public Feed feedly_feed { get; set; }

        public string id
        {
            get
            {
                return feedly_feed.id;
            }
            set
            {
                feedly_feed.id = value;
            }
        }

        public string title
        {
            get
            {
                return feedly_feed.title;
            }
            set
            {
                feedly_feed.title = value;
            }
        }

        public string url
        {
            get
            {
                return feedly_feed.website;
            }
            set
            {
                
            }
        }

        public string site_url
        {
            get
            {
                return feedly_feed.website;
            }
            set
            {
                feedly_feed.website = value;
            }
        }

        public System.Collections.ObjectModel.ObservableCollection<IItem> items
        {
            get;
            set;
        }

        public IAccount receiving_account
        {
            get;
            set;
        }

        public string icon_base64
        {
            get
            {
                return null;
            }
            set
            {
                
            }
        }

        public string icon_path
        {
            get
            {
                if (feedly_feed.website != null)
                {
                    return string.Format("https://plus.google.com/_/favicon?domain={0}&alt=feed", System.Web.HttpUtility.UrlEncode(feedly_feed.website));
                }
                return null;
            }
            set { }
        }

        public void update_items()
        {
            return;
        }

        public void mark_all_items_read()
        {
            FeedlyAccount account = this.receiving_account as FeedlyAccount;
            if(account != null) {
                RSSharp.Feedly.ApiCalls.Markers.mark_feed_as_read(account.token.access_token, this.feedly_feed.id);
                try
                {
                    IEnumerable<IItem> unread_items = account.unread_items.Where(item => item.feed_id == this.feedly_feed.id);
                    if (unread_items != null)
                    {
                        List<IItem> items = new List<IItem>();
                        foreach (IItem item in unread_items)
                        {
                            items.Add(item);
                        }
                        foreach (IItem item in items)
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

        public override string ToString()
        {
            return title;
        }
    }
}
