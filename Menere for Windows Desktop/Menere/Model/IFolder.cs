using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Menere.Model
{
    public interface IFolder
    {
        string name { get; set; }
        ObservableCollection<IFeed> feeds { get; set; }
        ObservableCollection<IFolder> sub_folders { get; set; }
        IFolder parent_folder { get; set; }

    }
}
