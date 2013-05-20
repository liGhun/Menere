using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFever.Model
{
    public class Item
    {
        public uint id { get; set; }       
        public uint feed_id { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public string html { get; set; }
        public string url { get; set; }
        public bool is_saved { get; set; }
        public bool is_read { get; set; }
        public ulong created_on_time { get; set; }

        public override string ToString()
        {
            return title;
        }
    }
}
