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

        public FeverFolder(SharpFever.Model.FeedsGroup fever_feeds_group, FeverFolder parent = null)
        {
            feeds = new System.Collections.ObjectModel.ObservableCollection<IFeed>();
            sub_folders = new ObservableCollection<IFolder>();
            parent_folder = parent_folder;
        }

        public System.Collections.ObjectModel.ObservableCollection<IFeed> feeds
        {
            get;
            set;
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
    }
}
