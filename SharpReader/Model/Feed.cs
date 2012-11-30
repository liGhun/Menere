using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ReaderSharp.Model
{
    public class Feed : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The name of the feed
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// The id of the feed
        /// </summary>
        public string id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        private string _id { get; set; }

        /// <summary>
        /// The subtitle / description of the feed
        /// </summary>
        public string SubTitle { get; set; }

        /// <summary>
        /// The link to the feed
        /// </summary>
        public string LinkSelf { get; set; }

        /// <summary>
        /// The link to the homepage of the feed
        /// </summary>
        public string LinkAlternate { get; set; }

        /// <summary>
        /// Last update time
        /// </summary>
        public DateTime updatedDateTime { get; set; }

        public long updated { get; set; }

        public string sortid { get; set; }
        public long firstitemmsec { get; set; }

        public ObservableCollection<Item> UnreadItems
        {
            get
            {
                return _unreadItems;
            }
            set
            {
                _unreadItems = value;
                NotifyPropertyChanged("UnreadItems");
                NotifyPropertyChanged("UnreadItemsCount");
                NotifyPropertyChanged("HasUnreadItems");
            }
        }

        private ObservableCollection<Item> _unreadItems { get; set; }
        public bool HasUnreadItems
        {
            get
            {
                if (UnreadItems.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public int UnreadItemsCount
        {
            get
            {
                return UnreadItems.Count();
            }
        }

        public ObservableCollection<Item> ReadItems
        {
            get
            {
                return _readItems;
            }
            set
            {
                _readItems = value;
                NotifyPropertyChanged("ReadItems");
                NotifyPropertyChanged("ReadItemsCount");
                NotifyPropertyChanged("HasReadItems");
            }
        }

        private ObservableCollection<Item> _readItems { get; set; }
        public bool HasReadItems
        {
            get
            {
                if (ReadItems.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public int ReadItemsCount
        {
            get
            {
                return ReadItems.Count();
            }
        }

        public string Icon
        {
            get {
                if (LinkAlternate != null)
                {
                    return LinkAlternate + "/favicon.ico";
                }
                else
                {
                    return null;
                }
            }
        }

        public override string ToString()
        {
            return title + " (" + UnreadItemsCount.ToString() + ")";
        }

        public Feed()
        {
            _unreadItems = new ObservableCollection<Item>();
        }

        #region NotifyProperty
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged == null)
            {
                PropertyChanged += new PropertyChangedEventHandler(Item_PropertyChanged);
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }
        #endregion

    }
}
