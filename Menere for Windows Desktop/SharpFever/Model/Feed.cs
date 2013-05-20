using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFever.Model
{
    public class Feed
    {
        public uint id { get; set; }
        public uint favicon_id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string site_url { get; set; }
        public bool is_spark { get; set; }
        public ulong last_updated_on_time { get; set; }

        public override string ToString()
        {
            return title;
        }
    }
}
