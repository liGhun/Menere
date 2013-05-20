using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFever.Model
{
    public class Group
    {
        public uint id { get; set; }
        public string title { get; set; }

        public override string ToString()
        {
            return title;
        }
    }
}
