using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppNetDotNet.Model;
using AppNetDotNet.ApiCalls;

namespace Menere.ExternalServices
{
    class AppDotNet : IExternalService
    {
        public string service_name
        {
            get { return "App.net"; }
        }

        public string name
        {
            get { return "@" + user.username; }
        }

        public string description
        {
            get { return "App.net is a social network with posts, messages and much more"; }
        }

        public string homepage
        {
            get { return "http://www.app.net/"; }
        }

        public string icon
        {
            get { throw new NotImplementedException(); }
        }

        public string settings
        {
            get { throw new NotImplementedException(); }
        }

        public IExternalService read_settings(string settings)
        {
            throw new NotImplementedException();
        }

        public void add_new_account()
        {
            Authorization.clientSideFlow apnClientAuthProcess = new AppNetDotNet.Model.Authorization.clientSideFlow("QPLF9Ypw94rWZhsjYVkeGaGkEBfzYrUh", "http://www.li-ghun.de/oauth/", "basic write_post");
            apnClientAuthProcess.AuthSuccess += apnClientAuthProcess_AuthSuccess;
            apnClientAuthProcess.showAuthWindow();
        }

        void apnClientAuthProcess_AuthSuccess(object sender, AppNetDotNet.AuthorizationWindow.AuthEventArgs e)
        {
            if (e != null)
            {
                if (e.success)
                {
                    if (!string.IsNullOrEmpty(e.accessToken))
                    {
                        Tuple<Token, ApiCallResponse> response = Tokens.get(e.accessToken);
                        if (response.Item2.success && response.Item1 != null)
                        {
                            if (response.Item1.user != null)
                            {
                                access_token = e.accessToken;
                                user = response.Item1.user;
                            }
                            //AppController.available_external_services.Add(this);
                        }

                    }
                }
            }
        }

        public void Send(Model.IItem iitem)
        {
            throw new NotImplementedException();
        }

        private string access_token {get;set;}
        private User user { get; set; }

    }
}
