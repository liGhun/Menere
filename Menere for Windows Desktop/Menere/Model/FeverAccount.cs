using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using SharpFever;
using SharpFever.Model;

namespace Menere.Model
{
    public class FeverAccount : IAccount
    {
        public FeverAccount()
        {
            fever_account = new SharpFever.Model.Account();
            feeds = new ObservableCollection<IFeed>();
            items = new ObservableCollection<IItem>();
        }


        private bool first_fetch_completed;
        public string url { get; set; }
        public SharpFever.Model.Account fever_account;
        public ObservableCollection<SharpFever.Model.FavIcon> favicons { get; set; }

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
                FeverResponse available_feeds = fever_account.get_feeds();
                foreach (SharpFever.Model.Feed feed in available_feeds.feeds)
                {
                    feeds.Add(new FeverFeed(feed, this));
                }
                FeverResponse available_favicons = fever_account.get_favicons();
                foreach (SharpFever.Model.FavIcon favicon in available_favicons.favicons)
                {
                    favicons.Add(favicon);
                }
            }
            return response.auth;
        }

        public string name
        {
            get;
            set;
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
            SharpFever.Model.FeverResponse unread_items_ids = fever_account.get_unread_item_ids();
            if (unread_items_ids != null)
            {
                if (unread_items_ids.unread_item_ids_list != null && !string.IsNullOrEmpty(unread_items_ids.unread_item_ids))
                {
                    SharpFever.Model.FeverResponse unread_fever_items = fever_account.get_items(with_ids: unread_items_ids.unread_item_ids_list);
                    foreach (SharpFever.Model.Item fever_item in unread_fever_items.items)
                    {
                        IItem existing_item = null;
                        try
                        {
                            existing_item = items.Where(item => item.id == fever_item.id.ToString()).First();
                        }
                        catch { }
                        if (existing_item == null)
                        {
                            Model.FeverItem item = new Model.FeverItem(fever_item, this);
                            item.receiving_account = this;
                            items.Add(item);
                            if (first_fetch_completed)
                            {
                                AppController.Current.snarl_interface.Notify(classId: "New unread item", title: item.feed.title, text: item.title,iconBase64:item.feed.icon_base64);
                            }
                        }
                    }
                }
            }
            first_fetch_completed = true;
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
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(FeverSettings));
            FeverSettings settings = new FeverSettings();
            settings = (FeverSettings)serializer.Deserialize(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(settings_string)));
            this.email = settings.email;
            this.api_key = settings.api_key;
            this.url = settings.url;
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

        public class FeverSettings
        {
            public string email { get; set; }
            public string api_key { get; set; }
            public string url { get; set; }
        }
    }
}
