using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Menere.Model
{
    public interface IAccount
    {
        string type { get;}
        string icon_path { get;}
        string name { get; set; }
        bool initial_fetch_completed { get; set; }
        ObservableCollection<IFeed> feeds { get; set; }
        ObservableCollection<IItem> items { get; set; }
        ObservableCollection<IItem> unread_items { get; set; }
        ObservableCollection<IItem> saved_items { get; set; }
        ObservableCollection<IFolder> groups { get; set; }

        string folder_name { get;}
        
        void load_settings(string settings);
        string get_settings();
        bool check_credentials();
        bool update_all_feeds();
        void add_new_account();
    }
}
