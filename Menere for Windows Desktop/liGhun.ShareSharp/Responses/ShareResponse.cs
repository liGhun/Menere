using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareSharp
{
    public class ShareResponse
    {
        // true if the item has been stored successfully
        public bool Success { get; set; }
        
        // the content of the HTTP response or the stacktrace
        public string ResponseText { get; set; }

        // the error text (if available)
        public string ErrorMessage { get; set; }

        // indicates that username and password are missing. application might want to open dialog to ask for it
        public bool UsernameOrPasswordMissing { get; set; }
    }
}
