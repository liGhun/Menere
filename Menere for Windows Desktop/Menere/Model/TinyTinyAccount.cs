using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menere.Model
{
    public class TinyTinyAccount : IAccount
    {
        public string type
        {
            get { return "Tiny Tiny RSS"; }
        }

        public string icon_path
        {
            get { return "/Menere;component/Images/AccountTypes/TinyTiny.png"; }
        }

        public string name
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
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public System.Collections.ObjectModel.ObservableCollection<IItem> items
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
            UserInterface.AccountSpecific.AccountTypeNotAvailable not_available = new UserInterface.AccountSpecific.AccountTypeNotAvailable();
            not_available.Show();
        }
    }
}
