using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace SharpFever.Model
{
    public class Link
    {
        public uint id { get; set; }
        public uint feed_id { get; set; }
        public uint item_id { get; set; }
        public float temperature { get; set; }
        public bool is_item { get; set; }
        public bool is_local { get; set; }
        public bool is_saved { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string item_ids { get; set; }
    }
}
