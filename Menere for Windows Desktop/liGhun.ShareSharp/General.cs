using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareSharp
{
    public class General
    {
        /// <summary>
        /// Every account has a method returning a KeyValuePair which holds storable data to load the account
        /// Pass this KeyValuePair here and you'll get back the account
        /// Take care that a null response could be returned
        /// Also take care if the Verified attribute is returning true - otherwise the settings loading completed but
        /// the data did not work (for example changed password)
        /// </summary>
        /// <param name="settings">The KeyValuePair as returned by the account</param>
        /// /// <param name="salt">A salt for the decyrption. Must be the same as the one having been used on encryption time on 
        /// settings retrieval</param>
        /// <returns></returns>
        public static IShareService load_settings(KeyValuePair<string, string> settings, string salt)
        {
            switch (settings.Key)
            {
                case "App.net":
                    AppNet appNet = new AppNet();
                    appNet.load_settings(settings.Value, salt);
                    return appNet;

                case "Delicious":
                    Delicious delicious = new Delicious();

                    return delicious;

                case "Instapaper":
                    Instapaper instapaper = new Instapaper();

                    return instapaper;

                case "Pocket":
                    Pocket pocket = new Pocket();
                    pocket.load_settings(settings.Value, salt);

                    return pocket;

                default:
                    return null;
            }

        }

    }
}
