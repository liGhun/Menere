using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReaderSharp.Model;
using ReaderSharp.Helper;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace ReaderSharp
{
    public class Reader
    {
        private const string GOOGLE_ADDRESS = "https://www.google.com/";
        private const string GOOGLE_API_ADDRESS = GOOGLE_ADDRESS + "reader/api/0/";
        private const string GOOGLE_LOGIN_ADDRESS = GOOGLE_ADDRESS + "accounts/ClientLogin";
        private const string GOOGLE_CMD_TOKEN = "token";
        private const string GOOGLE_CMD_EDIT_TAGS = "edit-tag";
        private const string GOOGLE_CMD_LIST_SUBSCRIPTIONS = "subscription/list";

        public long LastUpdateOfUpdateItems { get; set; }


        public string ClientName { get; set; }

        #region Create

        private Reader(string clientName)
        {
            this.ClientName = clientName;
        }

        public static Reader Login(string email, string password, string clientName)
        {

            Properties.Settings.Default.authToken = "";
            const string authUrl = GOOGLE_LOGIN_ADDRESS;

            var reader = new Reader(clientName);

            string response = HttpCommunications.SendPostRequest(authUrl, new
            {
                service = "reader",
                Email = email,
                Passwd = password,
                source = clientName,
                accountType = "GOOGLE"
            }, false, clientName);

            string authToken = "";

            try
            {
                authToken = new Regex(@"Auth=(?<authToken>\S+)").Match(response).Result("${authToken}");
                Properties.Settings.Default.authToken = authToken.Trim();
            }
            catch (Exception e)
            {
                throw new ArgumentException("AuthToken parsing error: " + e.Message);
            }

            return reader;
        }
        #endregion


        private string GetToken()
        {
            return HttpCommunications.SendGetRequest(GOOGLE_API_ADDRESS + GOOGLE_CMD_TOKEN, new { }, false).Trim();
        }

        #region API calling methods

        public ParserModels.UnreadCount GetUnreadCount()
        {
            return GetUnreadCount(DateTime.Now);
        }

        public ParserModels.UnreadCount GetUnreadCount(DateTime LastUpdate)
        {
            string content = HttpCommunications.SendGetRequest(@"https://www.google.com/reader/api/0/unread-count", new
            {
                all = "true",
                output = "json",
                ck = DateTime.Now.ToString(),
                client = ClientName
            }, false);
;
            ParserModels.UnreadCount json;
            try
            {
                json = JsonConvert.DeserializeObject<ParserModels.UnreadCount>(content);
            }
            catch (Exception exp)
            {
                return null;
            }            
            return json;
        }

        public ParserModels.ListOfFeeds GetAllSubscribedFeeds()
        {
            string content = HttpCommunications.SendGetRequest(@"https://www.google.com/reader/api/0/subscription/list", new
            {
                all = "true",
                output = "json",
                ck = DateTime.Now.ToString(),
                client = ClientName
            }, false);
;
            ParserModels.ListOfFeeds json;  
            try
            {
                json = JsonConvert.DeserializeObject<ParserModels.ListOfFeeds>(content);
            }
            catch (Exception exp)
            {
                return null;
            }
            
            return json;
            
        }

        public ParserModels.ListOfItems GetAllUnreadItems()
        {
            string content;

            if (LastUpdateOfUpdateItems == 0)
            {
                content = HttpCommunications.SendGetRequest(@"https://www.google.com/reader/api/0/stream/contents/user/-/state/com.google/reading-list", new
                {
                    r = "o",
                    n = 500,
                    xt = "user/-/state/com.google/read",
                    output = "json",
                    ck = DateTime.Now.ToString(),
                    client = ClientName
                }, false);
            }
            else
            {
                content = HttpCommunications.SendGetRequest(@"https://www.google.com/reader/api/0/stream/contents/user/-/state/com.google/reading-list", new
                {
                    r = "o",
                    n = 500,
                    xt = "user/-/state/com.google/read",
                    output = "json",
                    ck = DateTime.Now.ToString(),
                    client = ClientName,
                    ot = LastUpdateOfUpdateItems
                }, false);
            }
            
            ParserModels.ListOfItems json;
            try
            {
                json = JsonConvert.DeserializeObject<ParserModels.ListOfItems>(content);
            }
            catch (Exception exp)
            {
                return null;
            }
            LastUpdateOfUpdateItems = json.updated;
            return json;

        }

        #endregion
    }
}
