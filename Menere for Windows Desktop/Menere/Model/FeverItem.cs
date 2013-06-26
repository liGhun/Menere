using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menere.Model
{
    public class FeverItem : IItem
    {
        public SharpFever.Model.Item fever_item;

        public FeverItem(SharpFever.Model.Item item, IAccount account, FeverFeed feed)
        {
            fever_item = item;
            this.receiving_account = account;
            this.feed = feed;
        }

        public string id
        {
            get
            {
                return fever_item.id.ToString();
            }
            set
            {
                fever_item.id = Convert.ToUInt32(value);
            }
        }

        public string feed_id
        {
            get
            {
                return fever_item.feed_id.ToString();
            }
            set
            {
                fever_item.feed_id = Convert.ToUInt32(value);
            }
        }

        public IFeed feed
        {
            get;
            set;
        }

        public string title
        {
            get
            {
                return fever_item.title;
            }
            set
            {
                fever_item.title = value;
            }
        }

        public string html
        {
            get
            {
                return fever_item.html;
            }
            set
            {
                fever_item.html = value;
            }
        }

        public string author
        {
            get
            {
                return fever_item.author;
            }
            set
            {
                fever_item.author = value;
            }
        }

        public string url
        {
            get
            {
                return fever_item.url;
            }
            set
            {
                fever_item.url = value;
            }
        }

        public bool is_read
        {
            get
            {
                return fever_item.is_read;
            }
            set
            {
                fever_item.is_read = value;
            }
        }

        public bool is_saved
        {
            get
            {
                return fever_item.is_saved;
            }
            set
            {
                fever_item.is_saved = value;
            }
        }

        public DateTime created
        {
            get
            {
                long ticks = Convert.ToInt64(fever_item.created_on_time);
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                dtDateTime = dtDateTime.AddSeconds(ticks).ToLocalTime();

                return dtDateTime;
            }
            set
            {
                
            }
        }

        public bool mark_read()
        {
            FeverAccount rev_account = this.receiving_account as FeverAccount;
            if (rev_account != null)
            {
                return rev_account.fever_account.mark_item_as_read(this.fever_item);
            }
            return false;
        }

        public bool mark_unread()
        {
            return false;
        }


        public IAccount receiving_account
        {
            get;
            set;
        }


        public bool mark_saved()
        {
            FeverAccount rev_account = this.receiving_account as FeverAccount;
            if (rev_account != null)
            {
                bool success = rev_account.fever_account.mark_item_as_saved(this.fever_item);
                if (success)
                {
                    is_saved = true;
                }
                return success;
            }
            return false;
        }

        public bool mark_unsaved()
        {
            FeverAccount rev_account = this.receiving_account as FeverAccount;
            if (rev_account != null)
            {
                bool success = rev_account.fever_account.mark_item_as_unsaved(this.fever_item);
                if (success)
                {
                    is_saved = false;
                }
                return success;
            }
            return false;
        }


        public string tag_string
        {
            get {
                // Fever has no tags or simlar
                return "";
            }
        }
    }
}
