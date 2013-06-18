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
    /// Interaction logic for Add_Account.xaml
    /// </summary>
    public partial class Add_Account : Window
    {
        public Add_Account()
        {
            InitializeComponent();

            listbox_accounts.ItemsSource = AppController.available_account_types;
            AppController.accounts.CollectionChanged += accounts_CollectionChanged;
        }

        void accounts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (AppController.accounts.Count >= 1)
            {
                this.Close();
            }
        }
    }
}
