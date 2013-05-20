using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace SharpFever.Model
{
    public class FeverResponse
    {
        public uint api_version { get; set; }
        public bool auth { get; set; }
        public ulong last_refreshed_on_time { get; set; }
        public ulong total_items { get; set; }
        public string saved_items_ids { get; set; }

        public ObservableCollection<Group> groups { get; set; }
        public ObservableCollection<Feed> feeds { get; set; }
        public ObservableCollection<Item> items { get; set; }
        public ObservableCollection<FeedsGroup> feeds_groups { get; set; }
        public ObservableCollection<FavIcon> favicons { get; set; }
        public ObservableCollection<Link> links { get; set; }

        public string unread_item_ids
        {
            get
            {
                return _unread_item_ids;
            }
            set
            {
                _unread_item_ids = value;
                if (value != null)
                {
                    string[] values = value.Split(',');
                    unread_item_ids_list = new List<uint>();
                    foreach (string stringValue in values)
                    {
                        uint intValue = 0;
                        if (uint.TryParse(stringValue, out intValue))
                        {
                            unread_item_ids_list.Add(intValue);
                        }
                    }
                }
            }
        }
        private string _unread_item_ids { get; set; }
        public List<uint> unread_item_ids_list { get; private set; }

    }
}
