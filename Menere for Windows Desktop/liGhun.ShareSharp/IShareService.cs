using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareSharp
{
    public interface IShareService
    {
        string Name { get; }
        string Description { get; }
        string Homepage { get; }
        bool Verified { get; set; }
        string LastError { get; }
        string ServiceIcon { get; }
        string ApiKey { get; set; }

        bool NeedsApiKey { get; }
        VerificationResponse VerifyCredentials();
        ShareResponse SendNow(string title, string description, string url);
    }
}
