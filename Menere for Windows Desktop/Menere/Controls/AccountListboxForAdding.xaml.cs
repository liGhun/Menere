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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Menere.Model;

namespace Menere.Controls
{
    /// <summary>
    /// Interaction logic for AccountListboxForAdding.xaml
    /// </summary>
    public partial class AccountListboxForAdding : UserControl
    {
        public AccountListboxForAdding()
        {
            InitializeComponent();
        }

        private void button_add_account_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if(button != null) {
                IAccount account = button.DataContext as IAccount;
                if (account != null)
                {
                    account.add_new_account();
                }
            }
        }
    }
}
