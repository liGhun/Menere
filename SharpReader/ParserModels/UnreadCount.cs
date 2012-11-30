using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ReaderSharp.ParserModels
{
    public class UnreadCount
    {
        public int max { get; set; }
        public ObservableCollection<UnreadCountFeed> unreadcounts { get; set; }

        public UnreadCount()
        {
            unreadcounts = new ObservableCollection<UnreadCountFeed>();
        }

        public int countSum {
            get
            {
                if (unreadcounts != null)
                {
                    return unreadcounts.Sum(feed => feed.count);
                }
                else
                {
                    return 0;
                }
            }
    }

        public class UnreadCountFeed
        {
            public string id { get; set; }
            public int count { get; set; }
            public long newestItemTimestampUsec { get; set; }
        }

    }
}
