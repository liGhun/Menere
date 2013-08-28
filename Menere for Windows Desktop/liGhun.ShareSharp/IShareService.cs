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
        string ServiceIcon { get;
        }
        string ApiKey { get; set; }
        bool NeedsApiKey { get; }

        /// <summary>
        /// Returns a KeyValuePair containing the name of the service type in the key
        /// and the settings as an encrypted string in the value
        /// Pass this KeyValuePair to the load method in the general class to restore the account
        /// </summary>
        /// <param name="salt">A unique random string which is used for the encryprion which needs to be passed also on loading</param>
        /// <returns></returns>
        KeyValuePair<string, string> get_settings(string salt);
        VerificationResponse VerifyCredentials();
        ShareResponse SendNow(string title, string description, string url);
    }
}
