using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSSharp.Feedly.Model;
using RSSharp.Feedly.ApiCalls;
using System.ComponentModel;

namespace Menere.Model
{
    public class FeedlyAccount : IAccount
    {
        public FeedlyAccount()
        {
            items = new System.Collections.ObjectModel.ObservableCollection<IItem>();
            feeds = new System.Collections.ObjectModel.ObservableCollection<IFeed>();
            groups = new System.Collections.ObjectModel.ObservableCollection<IFolder>();
            unread_items = new System.Collections.ObjectModel.ObservableCollection<IItem>();
            saved_items = new System.Collections.ObjectModel.ObservableCollection<IItem>();

            backgroundWorker_update_entries = new BackgroundWorker();
            backgroundWorker_update_entries.WorkerReportsProgress = true;
            backgroundWorker_update_entries.WorkerSupportsCancellation = true;
            backgroundWorker_update_entries.DoWork += backgroundWorker_update_entries_DoWork;
            backgroundWorker_update_entries.RunWorkerCompleted += backgroundWorker_update_entries_RunWorkerCompleted;
            backgroundWorker_update_entries.ProgressChanged += backgroundWorker_update_entries_ProgressChanged;

        }

        public override string ToString()
        {
            return this.name;
        }

        public long? newer_than { get; set; }

        void backgroundWorker_update_entries_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e != null)
            {
                switch (e.ProgressPercentage)
                {
                    case 10:
                        FeedlyFeed feed = e.UserState as FeedlyFeed;
                        if (feed != null)
                        {
                            feeds.Add(feed);
                        }
                        break;

                    case 90:
                        FeedlyItem item = e.UserState as FeedlyItem;
                        if (item != null)
                        {
                            items.Add(item);

                        }
                        break;

                    case 91:
                        FeedlyItem unread_item = e.UserState as FeedlyItem;
                        if (unread_item != null)
                        {
                            unread_items.Add(unread_item);
                            if (initial_fetch_completed)
                            {
                                AppController.Current.snarl_interface.Notify(classId: "New unread item", title: unread_item.feed.title, text: unread_item.title, icon:unread_item.feed.icon_path);
                            }
                        }
                        break;

                    case 92:
                        FeedlyItem saved_item = e.UserState as FeedlyItem;
                        if (saved_item != null)
                        {
                            saved_items.Add(saved_item);
                        }
                        break;

                    case 100:
                        long? updated = e.UserState as long?;
                        if (updated != null)
                        {
                            newer_than = updated;
                        }
                        break;
                }
            }
        }

        void backgroundWorker_update_entries_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!initial_fetch_completed)
            {
                first_fetch_completed = true;
                initial_fetch_completed = true;
                AppController.Current.update_filter();
            }
        }

        void backgroundWorker_update_entries_DoWork(object sender, DoWorkEventArgs e)
        {
            Dictionary<string, FeedlyItem> known_items = new Dictionary<string, FeedlyItem>();
            Dictionary<string, FeedlyFeed> fetched_feeds = new Dictionary<string, FeedlyFeed>();

            // to not have being altered lists during foreach
            foreach (FeedlyItem item in this.items)
            {
                known_items.Add(item.id, item);
            }
            foreach (FeedlyFeed feed in this.feeds)
            {
                fetched_feeds.Add(feed.id, feed);
            }
            try
            {
                if (!first_fetch_completed)
                {
                    // fetching saved items first
                    Streams.entries_list saved_entries = Streams.get_entries_in_stream(this.token.access_token, string.Format("user/{0}/tag/global.saved", this.profile.id), count: 1000);
                    foreach (Entry entry in saved_entries.items)
                    {
                        FeedlyFeed feed_of_entry = null;
                        if (fetched_feeds.ContainsKey(entry.origin.streamId))
                        {
                            feed_of_entry = fetched_feeds[entry.origin.streamId];
                        }
                        else
                        {
                            Feed unknown_feed = RSSharp.Feedly.ApiCalls.Feeds.get(this.token.access_token, entry.origin.streamId);
                            if (unknown_feed != null)
                            {
                                feed_of_entry = new FeedlyFeed(this, unknown_feed);
                                fetched_feeds.Add(entry.origin.streamId, feed_of_entry);
                                backgroundWorker_update_entries.ReportProgress(10, feed_of_entry);
                            }
                            else
                            {
                                continue;
                            }
                        }

                        if (feed_of_entry != null)
                        {
                            FeedlyItem item = new FeedlyItem(this, feed_of_entry, entry);
                            item.is_saved = true;
                            backgroundWorker_update_entries.ReportProgress(90, item);
                            known_items.Add(item.id, item);
                            if (!item.is_read)
                            {
                                backgroundWorker_update_entries.ReportProgress(91, item);
                            }
                            backgroundWorker_update_entries.ReportProgress(92, item);
                        }
                    }

                    // now we are getting the unread ones
                    Streams.entries_list unread_entries = Streams.get_entries_in_stream(this.token.access_token, string.Format("user/{0}/category/global.all", this.profile.id), count: 1000, unread_only: true);
                    foreach (Entry entry in unread_entries.items)
                    {
                        if (known_items.ContainsKey(entry.id))
                        {
                            continue;
                        }

                        FeedlyFeed feed_of_entry = null;
                        if (fetched_feeds.ContainsKey(entry.origin.streamId))
                        {
                            feed_of_entry = fetched_feeds[entry.origin.streamId];
                        }
                        else
                        {
                            Feed unknown_feed = RSSharp.Feedly.ApiCalls.Feeds.get(this.token.access_token, entry.origin.streamId);
                            if (unknown_feed != null)
                            {
                                feed_of_entry = new FeedlyFeed(this, unknown_feed);
                                fetched_feeds.Add(entry.origin.streamId, feed_of_entry);
                                backgroundWorker_update_entries.ReportProgress(10, feed_of_entry);
                            }
                            else
                            {
                                continue;
                            }
                        }


                        if (feed_of_entry != null)
                        {
                            FeedlyItem item = new FeedlyItem(this, feed_of_entry, entry);
                            item.is_read = false;
                            known_items.Add(item.id, item);
                            backgroundWorker_update_entries.ReportProgress(90, item);
                            backgroundWorker_update_entries.ReportProgress(91, item);
                            if (entry.tags != null)
                            {
                                List<Tag> tags_saved = entry.tags.Where(t => t.id == string.Format("user/{0}/tag/global.saved", this.profile.id)) as List<Tag>;
                                if (tags_saved != null)
                                {
                                    if (tags_saved.Count > 0)
                                    {
                                        item.is_saved = true;
                                        backgroundWorker_update_entries.ReportProgress(92, item);
                                    }
                                }
                            }
                        }
                    }
                    

                    // finally we go through feeds for more items
                    foreach (FeedlyFeed feed in fetched_feeds.Values)
                    {
                        Streams.entries_list feed_entries = Streams.get_entries_in_stream(this.token.access_token, feed.id, count: 100);
                        foreach (Entry entry in feed_entries.items)
                        {
                            if (known_items.ContainsKey(entry.id))
                            {
                                continue;
                            }

                            FeedlyItem item = new FeedlyItem(this, feed, entry);
                            backgroundWorker_update_entries.ReportProgress(90, item);
                            known_items.Add(item.id, item);
                            if (!item.is_read)
                            {
                                backgroundWorker_update_entries.ReportProgress(91, item);
                            }
                            if (entry.tags != null)
                            {
                                List<Tag> tags_saved = entry.tags.Where(t => t.id == string.Format("user/{0}/tag/global.saved", this.profile.id)) as List<Tag>;
                                if (tags_saved != null)
                                {
                                    try
                                    {
                                        Tag tag = tags_saved.First();
                                        if (tags_saved.Count() > 0)
                                        {
                                            item.is_saved = true;
                                            backgroundWorker_update_entries.ReportProgress(92, item);
                                        }
                                    }
                                    catch (Exception exp) { 
                                        Console.WriteLine(exp.Message);
                                    }
                                }
                            }

                        }
                        if (feed_entries.updated > this.newer_than || newer_than == null)
                        {
                            backgroundWorker_update_entries.ReportProgress(100, feed_entries.updated);
                        }
                    }
                }
                else
                {
                    try
                    {
                        Streams.entries_list entries = Streams.get_entries_in_stream(this.token.access_token, string.Format("user/{0}/category/global.all", this.profile.id), count: 100, newer_than: newer_than);
                        foreach (Entry entry in entries.items)
                        {
                            if (known_items.ContainsKey(entry.id))
                            {
                                continue;
                            }


                            FeedlyFeed feed_of_entry = null;
                            if (fetched_feeds.ContainsKey(entry.origin.streamId))
                            {
                                feed_of_entry = fetched_feeds[entry.origin.streamId];
                            }
                            else
                            {
                                Feed unknown_feed = RSSharp.Feedly.ApiCalls.Feeds.get(this.token.access_token, entry.origin.streamId);
                                if (unknown_feed != null)
                                {
                                    feed_of_entry = new FeedlyFeed(this, unknown_feed);
                                    fetched_feeds.Add(entry.origin.streamId, feed_of_entry);
                                    backgroundWorker_update_entries.ReportProgress(10, feed_of_entry);
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            if (feed_of_entry != null)
                            {
                                FeedlyItem item = new FeedlyItem(this, feed_of_entry, entry);
                                backgroundWorker_update_entries.ReportProgress(90, item);
                                known_items.Add(item.id, item);
                            }

                            if (entries.updated > this.newer_than || newer_than == null)
                            {
                                backgroundWorker_update_entries.ReportProgress(100, entries.updated);
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        Console.WriteLine(exp.Message);
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }

            fetched_feeds = null;
            known_items = null;
        }

        private BackgroundWorker backgroundWorker_update_entries;

        private bool first_fetch_completed;
        private bool feed_fetch_completed;


        public RSSharp.Feedly.Model.Authentication.token token { get; set; }
        public RSSharp.Feedly.Model.Profile profile { get; set; }
        public string refresh_token { get; set; }

        public string type
        {
            get { return "Feedly"; }
        }

        public string icon_path
        {
            get { return "/Menere;component/Images/AccountTypes/feedly.png"; }
        }

        public string name
        {
            get
            {
                if (this.profile != null)
                {
                    return string.Format("Feedly {0}", this.profile.email);
                }
                else
                {
                    return "Feedly <unkonwn>";
                }
            }
            set { 
            
            }
        }

        public System.Collections.ObjectModel.ObservableCollection<IFeed> feeds
        {
            get;
            set;
        }

        public System.Collections.ObjectModel.ObservableCollection<IItem> items
        {
            get;
            set;
        }
        public System.Collections.ObjectModel.ObservableCollection<IItem> unread_items {get; set; }
        public System.Collections.ObjectModel.ObservableCollection<IItem> saved_items { get; set; }


        public void get_token_by_refresh_token() {
            token = RSSharp.Feedly.ApiCalls.Authentications.get_access_token_by_refresh_token(refresh_token, "svenwalther", "PETIPURZBLQRYEBVWJN06ZLH", "refresh_token");
        }

        public void load_settings(string settings_string)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(FeedlySettings));
            FeedlySettings settings = new FeedlySettings();
            settings = (FeedlySettings)serializer.Deserialize(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(settings_string)));
            this.token = settings.token;
            this.refresh_token = settings.refresh_token;
            this.get_token_by_refresh_token();
        }

        public string get_settings()
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(FeedlySettings));
            FeedlySettings settings = new FeedlySettings();
            settings.token = token;
            settings.profile = profile;
            settings.refresh_token = refresh_token;
            System.IO.MemoryStream memorywriter = new System.IO.MemoryStream();
            serializer.Serialize(memorywriter, settings);
            memorywriter.Flush();
            memorywriter.Position = 0;
            var sr = new System.IO.StreamReader(memorywriter);
            var myStr = sr.ReadToEnd();

            return myStr;
        }

        public bool check_credentials()
        {
            if (this.token == null)
            {
                return false;
            }
            this.profile = Profiles.get(this.token.access_token);
            if (profile != null)
            {
                get_all_categories();
                get_all_subscribed_feeds();
            }
            return (profile != null);
        }

        public void get_all_subscribed_feeds()
        {
            List<Subscription> subscriptions = Subscriptions.get(this.token.access_token);
            List<string> feed_ids = new List<string>();
            Dictionary<string,List<Category>> categories_in_subscription = new Dictionary<string,List<Category>>();
            foreach (Subscription subscription in subscriptions)
            {
                feed_ids.Add(subscription.id);
                if(subscription.categories != null) {
                    categories_in_subscription.Add(subscription.id,subscription.categories);
                }
            }
            List<Feed> available_feeds = Feeds.mget(this.token.access_token, feed_ids);
            if (available_feeds != null)
            {
                foreach (Feed feed in available_feeds)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(feed.title)) {
                            feed.title = "<no name>";
                        }
                        FeedlyFeed feedly_feed = new FeedlyFeed(this, feed);
                        if (categories_in_subscription[feed.id] != null)
                        {
                            foreach (Category category in categories_in_subscription[feed.id])
                            {
                                List<IFolder> cat_of_feed = groups.Where(g => g.name == category.label) as List<IFolder>;
                                if (cat_of_feed != null)
                                {
                                    if (cat_of_feed.Count > 0)
                                    {
                                        FeedlyFolder folder = cat_of_feed.First() as FeedlyFolder;
                                        folder.feeds.Add(feedly_feed);
                                    }
                                }
                            }
                        }

                        feeds.Add(feedly_feed);
                    }
                    catch (Exception exp)
                    {
                        Console.WriteLine(exp.Message);
                    }

                }
            }
            feed_fetch_completed = true;
        }

        public void get_all_categories()
        {
            List<Category> available_categories = Categories.get_all(this.token.access_token);
            foreach (Category category in available_categories)
            {
                FeedlyFolder folder = new FeedlyFolder(category);
                if (folder != null)
                {
                    groups.Add(folder);
                }
            }
        }


        public bool update_all_feeds()
        {
            if (!feed_fetch_completed)
            {
                return false;
            }
            if (!backgroundWorker_update_entries.IsBusy)
            {
                backgroundWorker_update_entries.RunWorkerAsync();
            }
            return true;
        }

        public void add_new_account()
        {
            //RSSharp.Feedly.Configuration.activate_sandbox();
            RSSharp.Feedly.ApiCalls.Authentication_window auth_window = new RSSharp.Feedly.ApiCalls.Authentication_window("svenwalther", "PETIPURZBLQRYEBVWJN06ZLH", "http://www.li-ghun.de/oauth");
            auth_window.AuthSuccess += auth_window_AuthSuccess;
            auth_window.Show();
            auth_window.startAuthorization();
        }

        void auth_window_AuthSuccess(object sender, RSSharp.Feedly.ApiCalls.Authentication_window.AuthEventArgs e)
        {
            if (e.success)
            {
                token = e.token;
                if (this.check_credentials())
                {
                    this.refresh_token = token.refresh_token;
                    AppController.accounts.Add(this);
                }
                else
                {
                    System.Windows.MessageBox.Show(e.error, "Error adding Feedly account");
                }
            }
            else
            {
                System.Windows.MessageBox.Show(e.error, "Error adding Feedly account");
            }
        }


        public System.Collections.ObjectModel.ObservableCollection<IFolder> groups
        {
            get;
            set;
        }

        public class FeedlySettings
        {
            public RSSharp.Feedly.Model.Authentication.token token { get; set; }
            public RSSharp.Feedly.Model.Profile profile { get; set; }
            public string refresh_token { get; set; }
        }

        private class cat_to_entry {
            public FeedlyItem item { get; set; }
            public FeedlyFolder folder { get; set; }
        }


        public bool initial_fetch_completed
        {
            get;
            set;
        }


        public string folder_name
        {
            get
            {
                return "Label";
            }

        }
    }
}
