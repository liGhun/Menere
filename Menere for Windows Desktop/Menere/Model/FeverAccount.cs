using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using SharpFever;
using SharpFever.Model;
using System.ComponentModel;

namespace Menere.Model
{
    public class FeverAccount : IAccount
    {
        public FeverAccount()
        {
            fever_account = new SharpFever.Model.Account();
            feeds = new ObservableCollection<IFeed>();
            items = new ObservableCollection<IItem>();
            groups = new ObservableCollection<IFolder>();
            unread_items = new ObservableCollection<IItem>();
            saved_items = new ObservableCollection<IItem>();

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


        private bool first_fetch_completed;
        public string url { get; set; }
        public SharpFever.Model.Account fever_account;
        public ObservableCollection<SharpFever.Model.FavIcon> favicons { get; set; }

        public bool initial_fetch_completed
        {
            get;
            set;
        }

        public bool check_credentials()
        {
            fever_account.api_key = this.api_key;
            fever_account.base_url = this.url;
            favicons = new ObservableCollection<FavIcon>();
            FeverResponse response = fever_account.check_credentials();
            if (response == null)
            {
                return false;
            }
            if (response.auth)
            {
                FeverResponse available_favicons = fever_account.get_favicons();
                foreach (SharpFever.Model.FavIcon favicon in available_favicons.favicons)
                {
                    favicons.Add(favicon);
                }
                FeverResponse available_feeds = fever_account.get_feeds();
                foreach (SharpFever.Model.Feed feed in available_feeds.feeds)
                {
                    feeds.Add(new FeverFeed(feed, this));
                }
                FeverResponse available_groups = fever_account.get_groups();
                foreach (SharpFever.Model.FeedsGroup group in available_groups.feeds_groups)
                {
                    groups.Add(new FeverFolder(group, feeds, available_groups.groups));
                }
            }
            return response.auth;
        }

        public string name
        {
            get
            {
                return string.Format("Fever {0}", this.email);
            }
            set { }
        }

        public ObservableCollection<IFeed> feeds
        {
            get;
            set;
        }

        public string email { get; set; }
        public string password { get; set; }
        public string api_key
        {
            get
            {
                if (!string.IsNullOrEmpty(stored_api_key))
                {
                    return stored_api_key;
                }
                else
                {
                    MD5 md5Hash = MD5.Create();
                    return GetMd5Hash(md5Hash, email + ":" + password);
                }

            }
            set
            {
                stored_api_key = value;
            }
        }
        public string stored_api_key { get; set; }


        public bool update_all_feeds()
        {
            if (!backgroundWorker_update_entries.IsBusy)
            {
                backgroundWorker_update_entries.RunWorkerAsync();
            }
            return true;
        }

        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }


        public void load_settings(string settings_string)
        {
            try
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(FeverSettings));
                FeverSettings settings = new FeverSettings();
                settings = (FeverSettings)serializer.Deserialize(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(settings_string)));
                this.email = settings.email;
                this.api_key = settings.api_key;
                this.url = settings.url;
            }
            catch { }
        }


        public string get_settings()
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(FeverSettings));
            FeverSettings settings = new FeverSettings();
            settings.email = this.email;
            settings.api_key = this.api_key;
            settings.url = this.url;
            System.IO.MemoryStream memorywriter = new System.IO.MemoryStream();
            serializer.Serialize(memorywriter, settings);
            memorywriter.Flush();
            memorywriter.Position = 0;
            var sr = new System.IO.StreamReader(memorywriter);
            var myStr = sr.ReadToEnd();

            return myStr;
        }

        public string type
        {
            get
            {
                return "Fever";
            }

        }

        public string icon_path
        {
            get
            {
                return "/Menere;component/Images/AccountTypes/fever.png";
            }

        }

        public void add_new_account()
        {
            UserInterface.AccountSpecific.AddNewFeverAccount add_account = new UserInterface.AccountSpecific.AddNewFeverAccount();
            add_account.Show();
        }


        public ObservableCollection<IItem> items
        {
            get;
            set;
        }

        public System.Collections.ObjectModel.ObservableCollection<IItem> unread_items { get; set; }
        public System.Collections.ObjectModel.ObservableCollection<IItem> saved_items { get; set; }

        public class FeverSettings
        {
            public string email { get; set; }
            public string api_key { get; set; }
            public string url { get; set; }
        }


        public ObservableCollection<IFolder> groups
        {
            get;
            set;
        }


        public string folder_name
        {
            get { return "Groups"; }
        }

        public uint max_id_fetched { get; set; }

        void backgroundWorker_update_entries_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e != null)
            {
                switch (e.ProgressPercentage)
                {
                    case 10:
                        FeverFeed feed = e.UserState as FeverFeed;
                        if (feed != null)
                        {
                            feeds.Add(feed);

                        }
                        break;

                    case 90:
                        FeverItem item = e.UserState as FeverItem;
                        if (item != null)
                        {
                            items.Add(item);

                        }
                        break;

                    case 91:
                        FeverItem unread_item = e.UserState as FeverItem;
                        if (unread_item != null)
                        {
                            unread_items.Add(unread_item);
                            if (initial_fetch_completed)
                            {
                                try
                                {
                                    AppController.Current.snarl_interface.Notify(classId: "New unread item", title: unread_item.feed.title, text: unread_item.title, icon: unread_item.feed.icon_path);
                                }
                                catch (Exception exp)
                                {
                                    AppController.add_debug_message(exp);
                                }
                            }
                        }
                        break;

                    case 92:
                        FeverItem saved_item = e.UserState as FeverItem;
                        if (saved_item != null)
                        {
                            saved_items.Add(saved_item);
                        }
                        break;

                        // unerad item in list which is not in the list by the API (external marked as read)
                    case 97:
                        FeverItem unread_item_which_is_not_unread_anymore = e.UserState as FeverItem;
                        if (unread_item_which_is_not_unread_anymore != null)
                        {
                            if(unread_items.Contains(unread_item_which_is_not_unread_anymore)) {
                                unread_items.Remove(unread_item_which_is_not_unread_anymore);
                            }
                        }
                        break;

                    case 99:
                        uint? last_updated = e.UserState as uint?;
                        if (last_updated != null)
                        {
                            this.fever_account.last_refreshed_on_time = last_updated.Value;
                        }
                        break;

                    case 100:
                        uint? updated = e.UserState as uint?;
                        if(updated != null) {
                            max_id_fetched = updated.Value;
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
            Dictionary<string, FeverItem> known_items = new Dictionary<string, FeverItem>();
            Dictionary<string, FeverFeed> fetched_feeds = new Dictionary<string, FeverFeed>();

            // to not have being altered lists during foreach
            foreach (FeverItem item in this.items)
            {
                known_items.Add(item.id, item);
            }
            foreach (FeverFeed feed in this.feeds)
            {
                fetched_feeds.Add(feed.id, feed);
            }

            #region first fetch
            if (!first_fetch_completed)
            {
                SharpFever.Model.FeverResponse unread_items_ids = fever_account.get_unread_item_ids();
                if (unread_items_ids != null)
                {
                    if (unread_items_ids.unread_item_ids_list.Count > 0)
                    {
                        while (unread_items_ids.unread_item_ids_list.Count > 0)
                        {
                            List<uint> ids = unread_items_ids.unread_item_ids_list.GetRange(0, Math.Min(unread_items_ids.unread_item_ids_list.Count, 50));
                            unread_items_ids.unread_item_ids_list.RemoveRange(0, Math.Min(unread_items_ids.unread_item_ids_list.Count, 50));
                            SharpFever.Model.FeverResponse unread_fever_items = fever_account.get_items(with_ids: ids);
                            foreach (SharpFever.Model.Item entry in unread_fever_items.items)
                            {
                                if (known_items.ContainsKey(entry.id.ToString()))
                                {
                                    continue;
                                }
                                FeverFeed feed_of_entry = null;
                                if (fetched_feeds.ContainsKey(entry.feed_id.ToString()))
                                {
                                    feed_of_entry = fetched_feeds[entry.feed_id.ToString()];
                                }
                                else
                                {
                                    FeverResponse available_feeds = fever_account.get_feeds();
                                    foreach (SharpFever.Model.Feed feed in available_feeds.feeds)
                                    {
                                        if (!fetched_feeds.ContainsKey(feed.id.ToString()))
                                        {
                                            FeverFeed new_feed = new FeverFeed(feed, this);
                                            fetched_feeds.Add(new_feed.id, new_feed);
                                            backgroundWorker_update_entries.ReportProgress(10, new_feed);
                                            if (new_feed.id == entry.feed_id.ToString())
                                            {
                                                feed_of_entry = new_feed;
                                            }
                                        }
                                    }
                                }

                                if (feed_of_entry == null)
                                {
                                    continue;
                                }



                                Model.FeverItem item = new Model.FeverItem(entry, this, feed_of_entry);
                                known_items.Add(item.id, item);
                                item.receiving_account = this;
                                backgroundWorker_update_entries.ReportProgress(90, item);

                                backgroundWorker_update_entries.ReportProgress(91, item);
                            }
                        }
                    }
                }
               // first_fetch_completed = true;
               // initial_fetch_completed = true;
            }
            #endregion

            uint max_id = this.max_id_fetched;
            FeverResponse entries = new FeverResponse();
            entries.items = new ObservableCollection<Item>();
            uint last_id = 0;
            uint current_id = 1;
              do {
                  entries = fever_account.get_items(since_id:Convert.ToInt32(max_id));
                  last_id = current_id;
                  current_id = max_id;
                  if (entries == null) { break; }
                    foreach (SharpFever.Model.Item entry in entries.items)
                    {
                        if(known_items.ContainsKey(entry.id.ToString())) 
                        {
                            continue;
                        }

                        FeverFeed feed_of_entry = null;
                        if (fetched_feeds.ContainsKey(entry.feed_id.ToString()))
                        {
                            feed_of_entry = fetched_feeds[entry.feed_id.ToString()];
                        }
                        else
                        {
                            FeverResponse available_feeds = fever_account.get_feeds();
                            foreach (SharpFever.Model.Feed feed in available_feeds.feeds)
                            {
                                if (!fetched_feeds.ContainsKey(feed.id.ToString()))
                                {
                                    FeverFeed new_feed = new FeverFeed(feed, this);
                                    fetched_feeds.Add(new_feed.id, new_feed);
                                    backgroundWorker_update_entries.ReportProgress(10, new_feed);
                                    if (new_feed.id == entry.feed_id.ToString())
                                    {
                                        feed_of_entry = new_feed;
                                    }
                                }
                            }
                        }

                        if (feed_of_entry == null)
                        {
                            continue;
                        }

                        FeverItem item = new FeverItem(entry,this, feed_of_entry);
                        known_items.Add(item.id, item);
                        item.receiving_account = this;

                        backgroundWorker_update_entries.ReportProgress(90, item);
                        if (!item.is_read)
                        {
                            backgroundWorker_update_entries.ReportProgress(91, item);
                        }
                        if (item.is_saved)
                        {
                            backgroundWorker_update_entries.ReportProgress(92, item);
                        }
                        if(entry.id > max_id) {
                            max_id = entry.id;
                            uint? report_value = entry.id;
                            backgroundWorker_update_entries.ReportProgress(100, report_value);
                        }

                    }
              } while (entries.items.Count > 0 && current_id != last_id);

            // check if another client has marked items as read in parallel
            FeverResponse check_number_unread_response = fever_account.get_unread_item_ids();
            if (check_number_unread_response != null)
            {
                if (check_number_unread_response.unread_item_ids_list != null)
                {
                    if (check_number_unread_response.unread_item_ids_list.Count < unread_items.Count && unread_items.Count < 1000)
                    {
                        // there are less available in the API than we know right now already
                        // 1000 is a limit by the API
                        foreach (IItem currently_seen_unread_item in unread_items)
                        {
                            if (!check_number_unread_response.unread_item_ids_list.Contains(Convert.ToUInt32(currently_seen_unread_item.id)))
                            {
                                // this one is not in the list so we will remove ist
                                try {
                                backgroundWorker_update_entries.ReportProgress(97, unread_items.Where(item => item.id == currently_seen_unread_item.id).First());
                                }
                                catch (Exception exp) {
                                    AppController.add_debug_message(exp);
                                }
                            }
                        }
                    }
                }
            }
        }

        private BackgroundWorker backgroundWorker_update_entries;

        ~FeverAccount()
        {
            try
            {
                saved_items.Clear();
                unread_items.Clear();
                items.Clear();
                if (backgroundWorker_update_entries.IsBusy)
                {
                    backgroundWorker_update_entries.CancelAsync();
                }
                backgroundWorker_update_entries = null;
                saved_items = null;
                unread_items = null;
                items = null;
            }
            catch { }
        }
    }
}
