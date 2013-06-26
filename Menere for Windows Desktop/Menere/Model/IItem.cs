using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menere.Model
{
    public interface IItem
    {
        string id { get; set; }
        string feed_id { get; set; }
        IFeed feed { get; set; }
        IAccount receiving_account { get; set; }
        string title { get; set; }
        string author { get; set; }
        string url { get; set; }
        string html { get; set; }
        bool is_read { get; set; }
        bool is_saved { get; set; }
        DateTime created { get; set; }
        string tag_string { get; }

        bool mark_read();
        bool mark_unread();
        bool mark_saved();
        bool mark_unsaved();
    }
}
