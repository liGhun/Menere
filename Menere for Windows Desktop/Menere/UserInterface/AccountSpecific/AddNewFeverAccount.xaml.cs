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
using Menere.Model;

namespace Menere.UserInterface.AccountSpecific
{
    /// <summary>
    /// Interaction logic for AddNewFeverAccount.xaml
    /// </summary>
    public partial class AddNewFeverAccount : Window
    {
        public AddNewFeverAccount()
        {
            InitializeComponent();
        }

        private void button_add_Click(object sender, RoutedEventArgs e)
        {
            FeverAccount account = new FeverAccount();
            account.email = textbox_username.Text;
            account.password = passwordbox_password.Password;
            account.url = textbox_url.Text + "?api";
            try
            {
                if (account.check_credentials())
                {
                    AppController.accounts.Add(account);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Your username or password is not valid", "Authorization failed", MessageBoxButton.OK);
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Authorization failed", MessageBoxButton.OK);
            }
            account = null;
        }

        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
