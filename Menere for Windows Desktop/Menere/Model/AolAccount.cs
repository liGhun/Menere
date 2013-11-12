using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSSharp.AOLreader.ApiCalls;
using RSSharp.AOLreader.Model;

namespace Menere.Model
{
    public class AolAccount: IAccount
    {
        private string client_id = "sv1KIWw70PvrIHhM";
        private string client_secret = "-HP7ph2BbKc20aGI0ntf";
        private string redirect_uri = "https://www.sven-walther.de/oauth/menere/";
        public Authentication.token token { get; set; }
        public string refresh_token { get; set; }

        public string type
        {
            get { return "AOL"; }
        }

        public string icon_path
        {
            get { return ""; }
        }

        public string name
        {
            get
            {
                return "AOL";
            }
            set
            {
                
            }
        }

        public bool initial_fetch_completed
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public System.Collections.ObjectModel.ObservableCollection<IFeed> feeds
        {
            get;
            set;
        }

        public System.Collections.ObjectModel.ObservableCollection<IItem> items
        {
            get;
            set;
        }

        public System.Collections.ObjectModel.ObservableCollection<IItem> unread_items
        {
            get;
            set;
        }

        public System.Collections.ObjectModel.ObservableCollection<IItem> saved_items
        {
            get;
            set;
        }

        public System.Collections.ObjectModel.ObservableCollection<IFolder> groups
        {
            get;
            set;
        }

        public string folder_name
        {
            get { throw new NotImplementedException(); }
        }

        public void load_settings(string settings)
        {
            throw new NotImplementedException();
        }

        public string get_settings()
        {
            throw new NotImplementedException();
        }

        public bool check_credentials()
        {
            if (this.token == null)
            {
                return false;
            }

            
            return true;
        }

        public bool update_all_feeds()
        {
            throw new NotImplementedException();
        }

        public void add_new_account()
        {
            RSSharp.AOLreader.ApiCalls.Authentication_window auth_window = new RSSharp.AOLreader.ApiCalls.Authentication_window(this.client_id, this.client_secret, this.redirect_uri);
            auth_window.AuthSuccess += auth_window_AuthSuccess;
            auth_window.Show();
            auth_window.startAuthorization();
        }

        void auth_window_AuthSuccess(object sender, RSSharp.AOLreader.ApiCalls.Authentication_window.AuthEventArgs e)
        {
            if (e.success)
            {
                token = e.token;
                if (this.check_credentials())
                {
                    this.refresh_token = token.refresh_token;
                    AppController.accounts.Add(this);
                }
                else
                {
                    System.Windows.MessageBox.Show(e.error, "Error adding AOL account");
                }
            }
            else
            {
                System.Windows.MessageBox.Show(e.error, "Error adding AOL account");
            }
        }
    }
}
