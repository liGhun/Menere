using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Menere.Model;
using Menere.UserInterface;

namespace Menere
{
    public class AppController
    {
        public static AppController Current;
        public static string app_data_path { get; set; }
        public static string app_program_path { get; set; }
        public static string themes_path { get; set; }
        public MainWindow main_window;
        private bool all_accounts_read { get; set; }
        public static ObservableCollection<IAccount> available_account_types { get; set; }
        public static ObservableCollection<IAccount> accounts { get; set; }
        public static ObservableCollection<MenereDebug> debug { get; set; }
        public static bool debug_enabled { get; set; }
        public Snarl.SnarlInterface snarl_interface;

        private string encryption_salt = "uioz/()&/(%890%F7>F$)obl OBg6ibIO/(b I(b7b&5)bOPlB/)5G(O)";

        public static ObservableCollection<ShareSharp.IShareService> available_external_services { get; set; }
        public static ObservableCollection<ShareSharp.IShareService> active_external_services { get; set; }

        public Model.IAccount current_account
        {
            get;
            set;
        }

        private AppController()
        {           
            Current = this;

            RSSharp.Common.HTTPCommunications.user_agent = "Menere " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            app_data_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\liGhun\\Menere\\";
            themes_path = app_data_path + "Themes\\";

            debug = new ObservableCollection<MenereDebug>();
            if(System.IO.File.Exists("C:\\debug.menere")) {
                debug_enabled = true;
                UserInterface.Debug debug_window = new Debug();
                debug_window.Show();

            }
            
            System.Windows.FrameworkElement.LanguageProperty.OverrideMetadata(typeof(System.Windows.FrameworkElement),
                new System.Windows.FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag)));


            available_account_types = new ObservableCollection<IAccount>();
            available_account_types.Add(new Model.FeverAccount());
            available_account_types.Add(new Model.FeedlyAccount());
           // available_account_types.Add(new Model.AolAccount());
           // available_account_types.Add(new Model.FeedbinAccount());
           // available_account_types.Add(new Model.TinyTinyAccount());
           // available_account_types.Add(new Model.CommaFeedAccount());

            accounts = new ObservableCollection<IAccount>();
            accounts.CollectionChanged += accounts_CollectionChanged;

            snarl_interface = new Snarl.SnarlInterface();
            snarl_interface.RegisterWithEvents("Meneré", "Meneré", "kjeshfrwiu87543o87");
            snarl_interface.AddClass("New unread item");

            if (!System.IO.Directory.Exists(themes_path))
            {
                System.IO.Directory.CreateDirectory(themes_path);
            }
            if (!System.IO.Directory.Exists(app_data_path + "\\icons"))
            {
                System.IO.Directory.CreateDirectory(app_data_path + "\\icons");
            }
            app_program_path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);

            AppNetDotNet.Model.Authorization.registerAppInRegistry(AppNetDotNet.Model.Authorization.registerBrowserEmulationValue.IE9Always);

            available_external_services = new ObservableCollection<ShareSharp.IShareService>();
            available_external_services.Add(new ShareSharp.AppNet());

            active_external_services = new ObservableCollection<ShareSharp.IShareService>();


            try
            {
                if (!Properties.Settings.Default.settings_updated)
                {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.settings_updated = true;
                }
            }
            catch
            {
                try
                {
                    Properties.Settings.Default.Reset();
                }
                catch { }
            }

            toggle_proxy();

            //Properties.Settings.Default.accounts = null;
            if (Properties.Settings.Default.accounts != null)
            {
                if (Properties.Settings.Default.accounts.Count > 0)
                {
                    foreach (string account_string in Properties.Settings.Default.accounts)
                    {
                        string[] account_data = account_string.Split(new string[] { "||||||||" }, StringSplitOptions.None);
                        if (account_data.Count() == 2)
                        {
                            switch (account_data[0])
                            {
                                case "Fever":
                                    FeverAccount fever_account = new FeverAccount();
                                    fever_account.load_settings(Helper.Crypto.ToInsecureString(Helper.Crypto.DecryptString(account_data[1])));
                                    if (fever_account.check_credentials())
                                    {
                                        accounts.Add(fever_account);
                                    }
                                    
                                    break;

                                case "Feedly":
                                    FeedlyAccount feedly_account = new FeedlyAccount();
                                    feedly_account.load_settings(Helper.Crypto.ToInsecureString(Helper.Crypto.DecryptString(account_data[1])));
                                    if (feedly_account.check_credentials())
                                    {
                                        accounts.Add(feedly_account);
                                        
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
                save_accounts();
            }

            all_accounts_read = true;

            if (Properties.Settings.Default.share_services != null)
            {
                if (Properties.Settings.Default.share_services.Count > 0)
                {
                    foreach (string service_string in Properties.Settings.Default.share_services)
                    {
                        string[] service_data = service_string.Split(new string[] { "||||||||" }, StringSplitOptions.None);
                        if (service_data.Count() == 2)
                        {
                            KeyValuePair<string, string> settings = new KeyValuePair<string, string>(service_data[0], service_data[1]);
                            ShareSharp.IShareService service = ShareSharp.General.load_settings(settings, encryption_salt);
                            if (service != null)
                            {
                                if (service.Verified)
                                {
                                    active_external_services.Add(service);
                                }
                            }
                        }
                    }
                }
            }

            if (accounts.Count == 0)
            {
                Add_Account add_account = new Add_Account();
                add_account.Show();
            }

        }


        ~AppController()
        {
            snarl_interface.Unregister();

            save_services();
        }

        private void save_accounts()
        {
            Properties.Settings.Default.accounts = new System.Collections.Specialized.StringCollection();
            foreach (IAccount to_be_saved_account in accounts)
            {
                string to_be_saved_string = to_be_saved_account.type + "||||||||" + Helper.Crypto.EncryptString(Helper.Crypto.ToSecureString(to_be_saved_account.get_settings()));
                Properties.Settings.Default.accounts.Add(to_be_saved_string);
            }

            Properties.Settings.Default.Save();
        }

        private void save_services()
        {
            Properties.Settings.Default.share_services = new System.Collections.Specialized.StringCollection();
            foreach (ShareSharp.IShareService service in active_external_services)
            {
                KeyValuePair<string, string> service_settings = service.get_settings(encryption_salt);
                string to_be_saved_service = service_settings.Key + "||||||||" + service_settings.Value;
                Properties.Settings.Default.share_services.Add(to_be_saved_service);
            }

            Properties.Settings.Default.Save();
        }

        void accounts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (main_window == null)
            {
                if (accounts.Count() > 0)
                {
                    this.current_account = accounts[0];
                }
                main_window = new MainWindow();
                main_window.Closing += main_window_Closing;
                toggle_listing();
            }
            
            main_window.combobox_accounts.ItemsSource = accounts;
            if (accounts.Count() > 0)
            {
                main_window.combobox_accounts.SelectedItem = accounts.Last();
                main_window.button_show_unread_Click(null, null);
                if (e.NewItems != null)
                {
                    foreach (IAccount added_account in e.NewItems)
                    {
                        added_account.update_all_feeds();
                    }
                }
            }
            if (main_window != null && accounts.Count > 0)
            {
                main_window.Show();
            }
            if (all_accounts_read)
            {
                save_accounts();
                if (accounts.Count() > 0)
                {
                    main_window.button_refresh_Click(null, null);
                }
            }
            if (accounts.Count > 1)
            {
                main_window.combobox_accounts.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                main_window.combobox_accounts.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void main_window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            App.Current.Shutdown();
        }

        public static void Start()
        {
            if (Current == null)
            {
                Current = new AppController();
            }
        }

        public void toggle_proxy()
        {
            if (Properties.Settings.Default.proxy_enabled)
            {
                System.Net.WebProxy myProxy = new System.Net.WebProxy(Properties.Settings.Default.proxy_server, Properties.Settings.Default.proxy_port);
                System.Net.WebRequest.DefaultWebProxy = myProxy;
                System.Net.HttpWebRequest.DefaultWebProxy = myProxy;
                System.Net.WebRequest.DefaultWebProxy = myProxy;
            }
            else
            {
                System.Net.WebRequest.DefaultWebProxy = null;
                System.Net.HttpWebRequest.DefaultWebProxy = null;
                System.Net.WebRequest.DefaultWebProxy = null;
            }

        }

        public void update_filter()
        {
            main_window.update_all_filter();
        }

        public static void add_debug_message(Exception exp = null)
        {
            if (debug_enabled)
            {
                debug.Add(new MenereDebug(exp.Message, text: exp.StackTrace, exp: exp));
            }
        }

        public static void add_debug_message(string title, string text = null) {
            if(debug_enabled) {
                debug.Add(new MenereDebug(title, text: text));
            }
        }

        public void toggle_listing()
        {
            if (Properties.Settings.Default.use_listView)
            {
                main_window.listbox_items.Visibility = System.Windows.Visibility.Collapsed;
                main_window.listview_items.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                main_window.listbox_items.Visibility = System.Windows.Visibility.Visible;
                main_window.listview_items.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
