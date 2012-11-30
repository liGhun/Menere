using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReaderSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ReaderSharp.Model;
using ReaderSharp.ParserModels;

namespace Menere
{
    public class AppController : INotifyPropertyChanged
    {
        public static AppController Current;
        public string UserAgent { get; private set; }
        public Reader CurrentReader { get; set; }

        public ObservableCollection<Feed> AllFeeds
        {
            get
            {
                return _allFeeds;
            }
            set
            {
                _allFeeds = value;
                NotifyPropertyChanged("AllFeeds");
                NotifyPropertyChanged("AllFeedsWithUnreadItems");
            }
        }
        private ObservableCollection<Feed> _allFeeds { get; set; }
        public ObservableCollection<Feed> AllFeedsWithUnreadItems
        {
            get;
            set;
        }
        public ObservableCollection<Item> AllUnreadItems { get; set; }

        public static void Start()
        {
            if (Current == null)
            {
                Current = new AppController();
            }
        }

        MainWindow mainWindow;

        private AppController()
        {
            if (Current == null)
            {
                Current = this;
                this.UserAgent = "Meneré 0.0.1 (http://www.li-ghun.de/) - early dev version";
                this.CurrentReader = Reader.Login("pidginsnarl@googlemail.com", "go105113", this.UserAgent);

                _allFeeds = new ObservableCollection<Feed>();
                AllFeedsWithUnreadItems = new ObservableCollection<Feed>();
                AllFeeds.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(AllFeeds_CollectionChanged);
                AllUnreadItems = new ObservableCollection<Item>();

                mainWindow = new MainWindow();

                UpdateAll();
                
                mainWindow.Show();
            }
        }

        public void UpdateAll()
        {
            if (AllFeeds.Count() == 0)
            {
                UpdateFeedSubsriptions();
            }
            UpdateItems();
        }

        public void UpdateItems()
        {
            UpdateUnreadItems();
        }

        public void UpdateFeedSubsriptions()
        {
            foreach (Feed feed in CurrentReader.GetAllSubscribedFeeds().subscriptions)
            {
                if (AllFeeds.Where(f => f.id == feed.id).Count() == 0)
                {
                    AllFeeds.Add(feed);
                }
            }
        }

        public void UpdateUnreadItems()
        {
            if (CurrentReader != null)
            {
                this.mainWindow.Title = "Meneré has " + CurrentReader.GetUnreadCount().countSum.ToString() + " unread items";


                foreach (Item item in CurrentReader.GetAllUnreadItems().items)
                {
                    if (AllUnreadItems.Where(i => i.id == item.id).Count() == 0)
                    {
                        Feed containingFeed = AllFeeds.Where(f => f.id == item.origin.streamId).First();
                        if (containingFeed != null)
                        {
                            item.ParentFeed = containingFeed;
                            containingFeed.UnreadItems.Add(item);
                           
                        }
                        
                    }
                }
            }
        }

        void AllFeeds_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e != null)
            {
                if (e.NewItems != null)
                {
                    foreach(Feed feed in e.NewItems) {
                        feed.UnreadItems.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(UnreadItems_CollectionChanged);
                    }
                }
            }   
        }

        void UnreadItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e != null)
            {
                if (e.NewItems != null)
                {
                    foreach (Item item in e.NewItems)
                    {
                        if (!AllFeedsWithUnreadItems.Contains(item.ParentFeed))
                        {
                            AllFeedsWithUnreadItems.Add(item.ParentFeed);
                        }
                        AllUnreadItems.Add(item);
                    }
                }
            }
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
