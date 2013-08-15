using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareSharp
{
    public class VerificationResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public bool UsernameOrPasswordMissing { get; set; }
    }
}
