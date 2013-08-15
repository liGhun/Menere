using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppNetDotNet.ApiCalls;
using AppNetDotNet.Model;

namespace ShareSharp
{
    public class AppNet : IShareService
    {
        public static event AccountAddedEventHandler AccountAddedSuccess;
        public delegate void AccountAddedEventHandler(object sender, AccountAddedEventArgs e);
        public class AccountAddedEventArgs : EventArgs
        {

            public bool success { get; set; }
            public AppNet account { get; set; }
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

        public static void add_new_account(string client_id)
        {
            Authorization.clientSideFlow apnClientAuthProcess = new AppNetDotNet.Model.Authorization.clientSideFlow(client_id, "http://www.li-ghun.de/oauth/", "basic write_post");
            apnClientAuthProcess.AuthSuccess += apnClientAuthProcess_AuthSuccess;
            apnClientAuthProcess.showAuthWindow();
        }

        static void apnClientAuthProcess_AuthSuccess(object sender, AppNetDotNet.AuthorizationWindow.AuthEventArgs e)
        {
            if (e != null)
            {
                if (e.success)
                {
                    if (!string.IsNullOrEmpty(e.accessToken))
                    {
                        AppNet account = new AppNet();
                        account.access_token = e.accessToken;
                        AccountAddedEventArgs event_args = new AccountAddedEventArgs();
                        event_args.success = true;
                        event_args.account = account;
                        AccountAddedSuccess(null, event_args);
                    }
                }
            }
        }

        public string access_token
        {
            get
            {
                return _access_token;
            }
            set
            {
                _access_token = value;
                if (!string.IsNullOrEmpty(_access_token))
                {
                    Tuple<Token, ApiCallResponse> response = Tokens.get(_access_token);
                    if (response.Item2.success)
                    {
                        token = response.Item1;
                        user = token.user;
                    }
                }
            }
        }
        private string _access_token { get; set; }
        private Token token { get; set; }

        public string Name
        {
            get {
                if (user == null)
                {
                    return "App.net";
                }
                else
                {
                    return "App.net @" + user.username;
                }
            }
        }

        public AppNetDotNet.Model.User user { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public string Description
        {
            get { return "App.net is a social network plattform"; }
        }

        public string Homepage
        {
            get { return "http://www.app.net/"; }
        }

        public bool Verified
        {
            get;
            set;
        }

        public string LastError
        {
            get { return ""; }
        }

        public string ServiceIcon
        {
            get { return ""; }
        }

        public string ApiKey
        {
            get;
            set;
        }

        public bool NeedsApiKey
        {
            get { return false; }
        }

        public VerificationResponse VerifyCredentials()
        {
            return new VerificationResponse();
        }

        public ShareResponse SendNow(string title, string description, string url)
        {
            AppNetDotNet.Model.Entities entities = new Entities();
            AppNetDotNet.Model.Entities.Link link = new Entities.Link();
            link.text = title;
            link.url = url;
            link.pos = 0;
            link.len = title.Length;
            entities.links.Add(link);
            AppNetDotNet.ApiCalls.Posts.create(this.access_token, title + " " + description, entities:entities);
            return new ShareResponse();
        }
    }
}
