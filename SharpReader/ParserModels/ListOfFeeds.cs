using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ReaderSharp.Model;

namespace ReaderSharp.ParserModels
{
    public class ListOfFeeds
    {
        public ObservableCollection<Feed> subscriptions { get; set; }
    }
}
