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
        public Preferences()
        {
            InitializeComponent();

            listbox_accounts.ItemsSource = AppController.accounts;
            listbox_external_services.ItemsSource = AppController.active_external_services;
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
    }
}
