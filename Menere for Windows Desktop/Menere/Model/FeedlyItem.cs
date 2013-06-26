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
            is_read = true;
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

        public bool is_saved
        {
            get;
            set;
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


        public bool mark_saved()
        {
            if (!is_saved)
            {
                FeedlyAccount account = this.receiving_account as FeedlyAccount;
                string save_id = string.Format("user/{0}/tag/global.saved", System.Web.HttpUtility.UrlEncode(account.profile.id));

                bool success = RSSharp.Feedly.ApiCalls.Tags.add_to_entry(account.token.access_token, this.feedly_entry.id, save_id);
                if (success)
                {
                    this.is_saved = true;
                }
                return success;
            }
            return true;
        }

        public bool mark_unsaved()
        {
            if (is_saved)
            {
                FeedlyAccount account = this.receiving_account as FeedlyAccount;
                string save_id = string.Format("user/{0}/tag/global.saved", account.profile.id);
                List<string> entries = new List<string>();
                entries.Add(this.feedly_entry.id);
                List<string> tags = new List<string>();
                tags.Add(save_id);

                bool success = RSSharp.Feedly.ApiCalls.Tags.delete_multiple_from_entries(account.token.access_token, entries, tags);
                if (success)
                {
                    this.is_saved = false;
                }
                return success;
            }
            return true;
        }


        public string tag_string
        {
            get {
                if(this.html.Contains("florist")) {
                    Console.WriteLine("bla");
                }

                if (feedly_entry.tags != null)
                {
                    string return_value = "";
                    foreach (Tag tag in feedly_entry.tags)
                    {
                        if (!tag.label.StartsWith("global."))
                        {
                            return_value += tag.label + ",";
                        }
                    }
                    char[] trim_chars = { ',', ' ' };
                    return_value = return_value.TrimEnd(trim_chars);
                    return return_value;
                }
                return "";
            }
        }
    }
}
