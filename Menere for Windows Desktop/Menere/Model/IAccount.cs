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
        ObservableCollection<IFeed> feeds { get; set; }
        ObservableCollection<IItem> items { get; set; }
        
        void load_settings(string settings);
        string get_settings();
        bool check_credentials();
        bool update_all_feeds();
        void add_new_account();
    }
}
