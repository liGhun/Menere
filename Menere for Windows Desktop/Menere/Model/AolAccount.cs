using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menere.Model
{
    public class AolAccount: IAccount
    {
        private string client_id = "sv1KIWw70PvrIHhM";
        private string client_secret = "-HP7ph2BbKc20aGI0ntf";
        private string redirect_uri = "https://www.sven-walther.de/oauth/menere/";

        public string type
        {
            get { return "AOL"; }
        }

        public string icon_path
        {
            get { return ""; }
        }

        public string name
        {
            get
            {
                return "AOL";
            }
            set
            {
                
            }
        }

        public bool initial_fetch_completed
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
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

        public System.Collections.ObjectModel.ObservableCollection<IItem> unread_items
        {
            get;
            set;
        }

        public System.Collections.ObjectModel.ObservableCollection<IItem> saved_items
        {
            get;
            set;
        }

        public System.Collections.ObjectModel.ObservableCollection<IFolder> groups
        {
            get;
            set;
        }

        public string folder_name
        {
            get { throw new NotImplementedException(); }
        }

        public void load_settings(string settings)
        {
            throw new NotImplementedException();
        }

        public string get_settings()
        {
            throw new NotImplementedException();
        }

        public bool check_credentials()
        {
            throw new NotImplementedException();
        }

        public bool update_all_feeds()
        {
            throw new NotImplementedException();
        }

        public void add_new_account()
        {
            throw new NotImplementedException();
        }
    }
}
