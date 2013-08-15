using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Menere.Model
{
    public class FeverFolder : IFolder
    {
        public SharpFever.Model.FeedsGroup fever_feeds_group { get; set; }
        public ObservableCollection<SharpFever.Model.Group> groups { get; set; }
        public ObservableCollection<IFeed> all_feeds { get; set; }

        public FeverFolder(SharpFever.Model.FeedsGroup fever_feeds_available, ObservableCollection<IFeed> feeds_available, ObservableCollection<SharpFever.Model.Group> groups_available, FeverFolder parent = null)
        {
            all_feeds = feeds_available;
            groups = groups_available;
            fever_feeds_group = fever_feeds_available;
            sub_folders = new ObservableCollection<IFolder>();
            parent_folder = parent_folder;
        }

        public override string ToString()
        {
            return groups.Where(gr => gr.id == this.fever_feeds_group.group_id).First().title;
        }

        public System.Collections.ObjectModel.ObservableCollection<IFeed> feeds
        {
            get
            {
                try
                {
                    IEnumerable<IFeed> the_feeds = all_feeds.Where(feed => this.fever_feeds_group.feed_ids_list.Contains(Convert.ToUInt32(feed.id)));
                    ObservableCollection<IFeed> return_value = new ObservableCollection<IFeed>();
                    if (the_feeds != null)
                    {
                        foreach (IFeed feed in the_feeds)
                        {
                            return_value.Add(feed);
                        }
                    }
                    return return_value;
                }
                catch (Exception exp)
                {
                    AppController.add_debug_message(exp);
                    return new System.Collections.ObjectModel.ObservableCollection<IFeed>();
                }
            }

            set { }
        }

        public System.Collections.ObjectModel.ObservableCollection<IFolder> sub_folders
        {
            get;
            set;
        }

        public IFolder parent_folder
        {
            get;
            set;
        }

        public string name
        {
            get
            {
                return this.ToString();
            }
            set
            {
                return;
            }
        }
    }
}
