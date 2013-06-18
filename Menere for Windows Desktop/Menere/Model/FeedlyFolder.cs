using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSSharp.Feedly.Model;

namespace Menere.Model
{
    public class FeedlyFolder : IFolder
    {
        public Category feedly_category { get; set; }

        public FeedlyFolder(Category category)
        {
            feeds = new System.Collections.ObjectModel.ObservableCollection<IFeed>();
            sub_folders = new System.Collections.ObjectModel.ObservableCollection<IFolder>();
            feedly_category = category;
        }

        public string name
        {
            get
            {
                return feedly_category.label;
            }
            set
            {
                feedly_category.label = value;
            }
        }

        public override string ToString()
        {
            return this.name;
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
