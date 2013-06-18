using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSSharp.Feedly.Model;

namespace Menere.Model
{
    class FeedlyItem : IItem
    {
        public Entry feedly_entry { get; set; }

        public FeedlyItem(IAccount account, FeedlyFeed feed, Entry entry)
        {
            feedly_entry = entry;
            this.feed_id = feed_id;
            this.receiving_account = account;
            this.feed = feed;
        }

        public string id
        {
            get
            {
                return feedly_entry.id;
            }
            set
            {
                feedly_entry.id = value;
            }
        }

        public string feed_id
        {
            get
            {
                if (feed != null)
                {
                    return feed.id;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                
            }
        }

        public IFeed feed
        {
            get;
            set;
        }

        public IAccount receiving_account
        {
            get;
            set;
        }

        public string title
        {
            get
            {
                if (feedly_entry.title != null)
                {
                    return feedly_entry.title;
                }
                return "";
            }
            set
            {
                feedly_entry.title = value;
            }
        }

        public string author
        {
            get
            {
                return feedly_entry.author;
            }
            set
            {
                feedly_entry.author = value;
            }
        }

        public string url
        {
            get
            {
                if (feedly_entry.alternate != null)
                {
                    if (feedly_entry.alternate.Count > 0)
                    {
                        if(!string.IsNullOrEmpty(feedly_entry.alternate[0].href)) {
                            return feedly_entry.alternate[0].href;
                        }
                    }
                }
                return "";
            }
            set
            {
                
            }
        }

        public string html
        {
            get
            {
                return feedly_entry.html;
            }
            set
            {
                feedly_entry.content = new Content();
                feedly_entry.content.content = value;
            }
        }

        public bool is_read
        {
            get
            {
                return !feedly_entry.unread;
            }
            set
            {
                feedly_entry.unread = !value;
            }
        }

        public DateTime created
        {
            get
            {
                try
                {
                    long ticks = Convert.ToInt64(feedly_entry.published / 1000);
                    System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    dtDateTime = dtDateTime.AddSeconds(ticks);
                    dtDateTime = dtDateTime.ToLocalTime();

                    return dtDateTime;
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.Message);
                    return DateTime.Now;
                }
            }
            set
            {
               
            }
        }

        public bool mark_read()
        {
            FeedlyAccount account = this.receiving_account as FeedlyAccount;
            return RSSharp.Feedly.ApiCalls.Markers.mark_entry_as_read(account.token.access_token, this.id);
        }

        public bool mark_unread()
        {
            return false;
        }
    }
}
