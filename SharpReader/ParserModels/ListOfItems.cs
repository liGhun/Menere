using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ReaderSharp.Model;

namespace ReaderSharp.ParserModels
{
    public class ListOfItems
    {
        public string id { get; set; }


        public string title { get; set; }

        public string continuation { get; set; }

        public string author { get; set; }

        public long updated { get; set; }

        public string direction { get; set; }

        public ObservableCollection<Item> items { get; set; }
    }
}
