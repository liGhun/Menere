using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSSharp.Feedly.Model;
using System.ComponentModel;

namespace Menere.Model
{
    class FeedlyItem : IItem, INotifyPropertyChanged
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
                NotifyPropertyChanged("is_read");
            }
        }

        public bool is_saved
        {
            get
            {
                return _is_saved;
            }
            set
            {
                _is_saved = value;
                NotifyPropertyChanged("is_saved");
            }
        }

        private bool _is_saved { get; set; }

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


        public void save_tags(string tag_string)
        {
            FeedlyAccount account = this.receiving_account as FeedlyAccount;
            if(account == null) {
                return;
            }

            if (string.IsNullOrWhiteSpace(tag_string))
            {
                if (this.feedly_entry.tags != null)
                {
                    List<string> tag_ids = new List<string>();
                    foreach (Tag tag in this.feedly_entry.tags)
                    {
                        tag_ids.Add(tag.id);
                    }
                    RSSharp.Feedly.ApiCalls.Tags.delete_multiple(account.token.access_token, tag_ids);
                    this.feedly_entry.tags.Clear();
                }
            }
            else
            {
                string[] tag_labels = tag_string.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (this.feedly_entry.tags == null)
                {
                    this.feedly_entry.tags = new List<Tag>();
                }

                List<string> to_be_added_tags = new List<string>();
                List<string> to_be_removed_tag_ids = new List<string>();
                foreach (Tag tag in this.feedly_entry.tags)
                {
                    if (tag_labels.Contains(tag.label))
                    {
                        // already existing tag - do nothing
                        continue;
                    }
                    else
                    {
                        to_be_removed_tag_ids.Add(tag.id);
                    }
                }

                foreach (string tag in tag_labels)
                {
                    IEnumerable<Tag> matches = feedly_entry.tags.Where(t => t.label == tag);
                    if (matches == null)
                    {
                        to_be_added_tags.Add(tag);
                    }
                    else
                    {
                        if (matches.Count() == 0)
                        {
                            to_be_added_tags.Add(tag);
                        }
                    }
                }

                if (to_be_removed_tag_ids.Count > 0)
                {
                    RSSharp.Feedly.ApiCalls.Tags.delete_multiple(account.token.access_token, to_be_removed_tag_ids);
                }

                if (to_be_added_tags.Count() > 0)
                {
                    List<string> tag_ids = new List<string>();
                    foreach (string label in to_be_added_tags)
                    {
                        tag_ids.Add(RSSharp.Feedly.Model.Tag.get_tag_id_for_label(label, account.profile.id));
                    }
                    RSSharp.Feedly.ApiCalls.Tags.add_multiple_to_entry(account.token.access_token, this.feedly_entry.id, tag_ids);
                }

                Entry changed_item = RSSharp.Feedly.ApiCalls.Entries.get(account.token.access_token, feedly_entry.id);
                this.feedly_entry.tags = changed_item.tags;

            }
        }

        #region Property changed
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property_name = "")
        {
            if (PropertyChanged != null && !string.IsNullOrEmpty(property_name))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property_name));
            }
        }
        #endregion
    }
}
