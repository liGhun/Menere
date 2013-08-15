using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menere.Model
{
    public class MenereDebug
    {
        public MenereDebug(string title, string text = null, Exception exp = null)
        {
            this.timestamp = DateTime.Now.ToLocalTime();
            this.debug_text = text;
            this.debug_title = title;
            this.exception = exp;
        }

        public DateTime timestamp { get; set; }
        public string debug_title { get; set; }
        public string debug_text { get; set; }
        public Exception exception { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ::: {1} ::: {2}", timestamp.ToLongTimeString(), debug_title, debug_text);
        }
    }
}
