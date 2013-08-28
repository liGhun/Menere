using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Menere.UserInterface
{
    /// <summary>
    /// Interaction logic for Preferences.xaml
    /// </summary>
    public partial class Preferences : Window
    {
        private string api_key_pocket = "17595-9d31cbe03df13b5380136353";

        public Preferences()
        {
            InitializeComponent();

            listbox_accounts.ItemsSource = AppController.accounts;
            listbox_external_services.ItemsSource = AppController.active_external_services;

            checkbox_use_list_view.IsChecked = Properties.Settings.Default.use_listView;

            richtTextBox_credits.AddHandler(Hyperlink.RequestNavigateEvent, new RoutedEventHandler(this.HandleRequestNavigate));
        }

        private void HandleRequestNavigate(object sender, RoutedEventArgs args)
        {
            if (args is System.Windows.Navigation.RequestNavigateEventArgs)
                System.Diagnostics.Process.Start(((System.Windows.Navigation.RequestNavigateEventArgs)args).Uri.AbsoluteUri);
        }

        private void button_add_app_net_Click(object sender, RoutedEventArgs e)
        {
            ShareSharp.AppNet.AccountAddedSuccess += AppNet_AccountAddedSuccess;
            ShareSharp.AppNet.add_new_account("QPLF9Ypw94rWZhsjYVkeGaGkEBfzYrUh");
        }

        void AppNet_AccountAddedSuccess(object sender, ShareSharp.AppNet.AccountAddedEventArgs e)
        {
            if (e.success)
            {
                AppController.active_external_services.Add(e.account);
            }
        }

        private void button_add_pocket_Click_1(object sender, RoutedEventArgs e)
        {
            ShareSharp.Pocket.AccountAddedSuccess += Pocket_AccountAddedSuccess;
            ShareSharp.Pocket.add_new_account(api_key_pocket, "http://www.dogfoodsoft.de/oauth/menere/pocket/");
        }

        void Pocket_AccountAddedSuccess(object sender, ShareSharp.Pocket.AccountAddedEventArgs e)
        {
            if (e.success)
            {
                AppController.active_external_services.Add(e.account);
            }
        }

        private void checkbox_use_list_view_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.use_listView = true;
            AppController.Current.toggle_listing();
        }

        private void checkbox_use_list_view_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.use_listView = false;
            AppController.Current.toggle_listing();
        }

        private void listbox_accounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Model.IAccount account = listbox_accounts.SelectedItem as Model.IAccount;
            if (account != null)
            {
                button_remove_rss_account.IsEnabled = true;
            }
            else
            {
                button_remove_rss_account.IsEnabled = false;
            }

        }

        private void button_add_feedly_Click(object sender, RoutedEventArgs e)
        {
            Model.FeedlyAccount feedly_account = new Model.FeedlyAccount();
            feedly_account.add_new_account();
        }

        private void button_add_fever_Click(object sender, RoutedEventArgs e)
        {
            Model.FeverAccount fever_account = new Model.FeverAccount();
            fever_account.add_new_account();
        }

        private void button_remove_rss_account_Click(object sender, RoutedEventArgs e)
        {
            Model.IAccount account = listbox_accounts.SelectedItem as Model.IAccount;
            if (account != null)
            {
                AppController.accounts.Remove(account);
                account = null;
            }
        }

        private void listbox_external_services_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShareSharp.IShareService service = listbox_external_services.SelectedItem as ShareSharp.IShareService;
            if (service != null)
            {
                button_remove_account.IsEnabled = true;
            }
            else
            {
                button_remove_account.IsEnabled = false;
            }
        }

        private void button_remove_account_Click(object sender, RoutedEventArgs e)
        {
            ShareSharp.IShareService service = listbox_external_services.SelectedItem as ShareSharp.IShareService;
            if (service != null)
            {
                AppController.active_external_services.Remove(service);
            }
        }
    }
}
