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

namespace Menere.UserInterface.AccountSpecific
{
    /// <summary>
    /// Interaction logic for AccountTypeNotAvailable.xaml
    /// </summary>
    public partial class AccountTypeNotAvailable : Window
    {
        public AccountTypeNotAvailable()
        {
            InitializeComponent();
        }

        private void button_ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
