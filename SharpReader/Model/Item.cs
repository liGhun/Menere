using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ReaderSharp.Model
{
    public class Item
    {
        /// <summary>
        /// THe title of the item
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// The summary (and most times content) of the item
        /// </summary>
        public Content summary { get; set; }

        /// <summary>
        /// The id of the item
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Time when this item has been published
        /// </summary>
        public long published { get; set; }

        public long timestampUsec { get; set; }

        /// <summary>
        /// Time when this item 
        /// </summary>
        public long updated { get; set; }

        public long crawlTimeMsec { get; set; }

        public Content content { get; set; }

        /// <summary>
        /// When has this item been crawled
        /// </summary>
        public DateTime crawledTime { get; set; }

        /// <summary>
        /// The author of the item (if known)
        /// </summary>
        public string author { get; set; }

        /// <summary>
        /// Is read stated locked?
        /// </summary>
        public bool readStateIsLocked { get; set; }

        public ObservableCollection<Link> alternate { get; set; }

        public Feed ParentFeed { get; set; }

        public string FullVersionUrl
        {
            get
            {
                if (alternate.Count > 0)
                {
                    return alternate[0].Url;
                }
                else
                {
                    // xxx
                    return "http://www.li-ghun.de/";
                }
            }
        }

        public Origin origin { get; set; }

        public enum ContentTypes
        {
            Html
        }

        public override string ToString()
        {
            return title;
        }

        public Item()
        {
            origin = new Origin();
       //     title = new Content();
            summary = new Content();
        }

        public class Content
        {
            /// <summary>
            /// The text itself
            /// </summary>
            public string content { get; set; }

            /// <summary>
            /// Type of the content
            /// </summary>
            public ContentTypes contentType { get; set; }

            public string direction { get; set; }

            /// <summary>
            /// The base for relative Urls in this item
            /// </summary>
            public string BaseUrl { get; set; }

            public Content()
            {
                contentType = ContentTypes.Html;
            }
        }

        public class Link
        {
            public string Url { get; set; }
            public string Type { get; set; }
            public string Rel { get; set; }
        }

        public class Origin
        {
            public string streamId { get; set; }
            public string title { get; set; }
            public string htmlUrl { get; set; }
        }
    }
}
