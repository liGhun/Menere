using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Menere.Model
{
    public interface IFeed
    {
        string id { get; set; }
        string title { get; set; }
        string url { get; set; }
        string site_url { get; set; }
        ObservableCollection<IItem> items { get; set; }
        IAccount receiving_account { get; set; }
        string icon_base64 { get; set; }
        string icon_path { get; set; }

        void update_items();
        void mark_all_items_read();
    }
}
