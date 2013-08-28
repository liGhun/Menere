using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ShareSharp
{
    public class Pocket : IShareService
    {
        public static event AccountAddedEventHandler AccountAddedSuccess;
        public delegate void AccountAddedEventHandler(object sender, AccountAddedEventArgs e);
        public class AccountAddedEventArgs : EventArgs
        {

            public bool success { get; set; }
            public Pocket account { get; set; }
            public string error { get; set; }
        }

        private static event AuthEventHandler AuthSuccess;
        private delegate void AuthEventHandler(object sender, AuthEventArgs e);
        private class AuthEventArgs : EventArgs
        {

            public bool success { get; set; }
            public string accessToken { get; set; }
            public string error { get; set; }
        }

        public static void add_new_account(string api_key, string redirect_uri)
        {

            WebHelpers.WebHelpers.Response response = WebHelpers.WebHelpers.SendPostRequest("Share#",
                @"https://getpocket.com/v3/oauth/request", 
                new
                        {
                            consumer_key = api_key,
                            redirect_uri = System.Web.HttpUtility.UrlEncode(redirect_uri)
                        }, 
                        false);

            if(response.Success) {
                if(Regex.Match(response.Content,"^code=.*").Success) {
                    MatchCollection matches = Regex.Matches(response.Content, "^code=(?<code>.*$)");
                    string code = matches[0].Groups["code"].Value;

                    string auth_url = string.Format("https://getpocket.com/auth/authorize?request_token={0}&redirect_uri={1}",System.Web.HttpUtility.UrlEncode(code), System.Web.HttpUtility.UrlEncode(redirect_uri));

                    System.Diagnostics.Process.Start(auth_url);

                    System.Windows.Forms.DialogResult dialog_result = System.Windows.Forms.MessageBox.Show("Your default browser will now be opened and forwarded to the Pocket authorization homepage. Please login there and authorize this app. When finished in your browser please get back to this dialog here and press the \"OK\" button", "Pocket authorization in progress",
                        System.Windows.Forms.MessageBoxButtons.OKCancel,
                        System.Windows.Forms.MessageBoxIcon.Information,
                        System.Windows.Forms.MessageBoxDefaultButton.Button1);

                    if (dialog_result == System.Windows.Forms.DialogResult.OK)
                    {
                        WebHelpers.WebHelpers.Response response_auth = WebHelpers.WebHelpers.SendPostRequest("Share#",
                            @"https://getpocket.com/v3/oauth/authorize",
                            new
                            {
                                consumer_key = api_key,
                                code = System.Web.HttpUtility.UrlEncode(code)
                            },
                            false);

                        if (response_auth.Success)
                        {
                            if (Regex.Match(response_auth.Content, "^access_token=.*").Success)
                            {
                                MatchCollection matches_auth = Regex.Matches(response_auth.Content, "^access_token=(?<token>[^&]*)&username=(?<username>.*)$");
                                string token = matches_auth[0].Groups["token"].Value;
                                string username = matches_auth[0].Groups["username"].Value;

                                Pocket account = new Pocket();
                                account.access_token = token;
                                account.Username = username;
                                account.consumer_key = api_key;
                                AccountAddedEventArgs event_args = new AccountAddedEventArgs();
                                event_args.success = true;
                                event_args.account = account;
                                AccountAddedSuccess(null, event_args);
                            }
                        }
                    }
                    
                }
            }
        }

        static void apnClientAuthProcess_AuthSuccess(object sender, AppNetDotNet.AuthorizationWindow.AuthEventArgs e)
        {
            if (e != null)
            {
                if (e.success)
                {
                    if (!string.IsNullOrEmpty(e.accessToken))
                    {
                        
                    }
                }
            }
        }

        public string ApiKey { get; set; }
        public string access_token { get; set; }
        public string consumer_key { get; set; }
        
        private string lastError
        {
            get
            {
                return lastError;
            }
        }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(Username))
                {
                    return "Pocket";
                }
                else
                {
                    return "Pocket: " + Username;
                }
            }

        }

        public override string ToString()
        {
            return Name;
        }

        public string Description
        {
            get
            {
                return "Pocket is a service to remember websites. See http://getpocket.com/";
            }
        }

        public string Homepage
        {
            get
            {
                return "http://getpocket.com/";
            }
        }

        public string ServiceIcon 
        {
            get
            {
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase), "Images", "ExternalServices", "pocket.png").Substring(6);
            }
        }

        public string ServiceIconRelativePath
        {
            get
            {
                return "Images\\ExternalServices\\pocket.png";
            }
        }

        public bool UsesOAuth
        {
            get
            {
                return false;
            }
        }

        public string Username
        {
            get;
            set;
        }

        public string Password
        {
            get
            {
                return Crypto.ToInsecureString(Crypto.DecryptString(Properties.Settings.Default.PocketPassword));
            }
            set
            {
                Properties.Settings.Default.PocketPassword = Crypto.EncryptString(Crypto.ToSecureString(value));
            }
        }

        public bool Verified
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(access_token) && !string.IsNullOrWhiteSpace(consumer_key));

            }
            set { } 
        }

        public string LastError
        {
            get
            {
                return lastError;
            }
        }

        public bool CanVerifyCredentials { get { return true; } }

        public VerificationResponse VerifyCredentials()
        {
            return new VerificationResponse();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public ShareResponse SendNow(string title, string description, string url)
        {
            ShareResponse shareResponse = new ShareResponse();
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    shareResponse.Success = false;
                    shareResponse.ErrorMessage = "No URL specified";
                }
                else
                {
                    if (Verified)
                    {
                        WebHelpers.WebHelpers.Response result;
                        result = WebHelpers.WebHelpers.SendPostRequest("Share#", @"https://getpocket.com/v3/add", new
                        {
                            consumer_key = this.consumer_key,
                            access_token = this.access_token,
                            url = url,
                            title = title,

                        }, false);

                        //result.Success = (!string.IsNullOrEmpty(result.Content) && result.Content.ToLowerInvariant() == "200 ok");
                        if (!result.Success)
                        {
                            shareResponse.Success = false;
                            shareResponse.ErrorMessage = result.Content;
                        }
                        else
                        {
                            shareResponse.Success = true;
                        }
                    }
                    else
                    {
                        shareResponse.Success = false;
                        shareResponse.ErrorMessage = "Unverified token";
                        shareResponse.UsernameOrPasswordMissing = true;
                        Verified = false;
                    }
                }
            }
            catch (Exception exp)
            {
                shareResponse.Success = false;
                shareResponse.ErrorMessage = exp.Message;
                shareResponse.ResponseText = exp.StackTrace;
            }
            return shareResponse;
        }


        public bool NeedsApiKey
        {
            get { return true; }
        }


        public KeyValuePair<string, string> get_settings(string salt)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(pocket_settings));
            pocket_settings settings = new pocket_settings();
            settings.access_token = this.access_token;
            settings.username = this.Username;
            settings.consumer_key = this.consumer_key;
            System.IO.MemoryStream memorywriter = new System.IO.MemoryStream();
            serializer.Serialize(memorywriter, settings);
            memorywriter.Flush();
            memorywriter.Position = 0;
            System.IO.StreamReader sr = new System.IO.StreamReader(memorywriter);
            string settings_string = sr.ReadToEnd();

            Crypto.set_salt(salt);
            settings_string = Crypto.EncryptString(Crypto.ToSecureString(settings_string));

            return new KeyValuePair<string, string>("Pocket", settings_string);
        }

        public void load_settings(string enycrpted_settings_string, string salt)
        {
            // Init XML reader and settings class
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(pocket_settings));
            pocket_settings settings = new pocket_settings();

            // Decrypt the string
            Crypto.set_salt(salt);
            string settings_string = Crypto.ToInsecureString(Crypto.DecryptString(enycrpted_settings_string));

            // load settings
            // note: by setting the access token Verified is set automatically in this class
            settings = (pocket_settings)serializer.Deserialize(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(settings_string)));
            this.access_token = settings.access_token;
            this.consumer_key = settings.consumer_key;
            this.Username = settings.username;
            this.VerifyCredentials();
        }

        public class pocket_settings
        {
            public string access_token { get; set; }
            public string consumer_key { get; set; }
            public string username { get; set; }
        }
    }
}
