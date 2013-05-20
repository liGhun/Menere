using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFever.Model
{
    public class FeedsGroup
    {
        public ulong group_id { get; set; }
        public string feeds_ids
        {
            get
            {
                return _feed_ids;
            }
            set
            {
                _feed_ids = value;
                if (value != null)
                {
                    string[] values = value.Split(',');
                    feed_ids_list.Clear();
                    foreach (string stringValue in values)
                    {
                        uint intValue = 0;
                        if(uint.TryParse(stringValue,out intValue)) {
                            feed_ids_list.Add(intValue);
                        }
                    }
                }
            }
        }
        private string _feed_ids { get; set; }
        public List<uint> feed_ids_list { get; private set; }

        public FeedsGroup()
        {
            feed_ids_list = new List<uint>();
        }
    }
}
