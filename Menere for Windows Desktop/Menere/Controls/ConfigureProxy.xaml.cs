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

namespace Menere.Controls
{
    /// <summary>
    /// Interaction logic for ConfigureProxy.xaml
    /// </summary>
    public partial class ConfigureProxy : UserControl
    {
        public ConfigureProxy()
        {
            InitializeComponent();

            checkbox_enable_proxy.IsChecked = Properties.Settings.Default.proxy_enabled;
            textbox_proxy_server.Text = Properties.Settings.Default.proxy_server;
            integerUpDown_proxy_port.Value = Properties.Settings.Default.proxy_port;
            textbox_proxy_username.Text = Properties.Settings.Default.proxy_username;
            passwordbox_proxy_password.Password = Helper.Crypto.ToInsecureString(Helper.Crypto.DecryptString(Properties.Settings.Default.proxy_password));
        }

        private void checkbox_enable_proxy_Checked(object sender, RoutedEventArgs e)
        {
            grid_proxy_settings.Visibility = System.Windows.Visibility.Visible;
            set_proxy();
        }

        private void checkbox_enable_proxy_Unchecked(object sender, RoutedEventArgs e)
        {
            grid_proxy_settings.Visibility = System.Windows.Visibility.Collapsed;
            set_proxy();
        }

        private void set_proxy()
        {
            Properties.Settings.Default.proxy_enabled = checkbox_enable_proxy.IsChecked.Value;
            Properties.Settings.Default.proxy_server = textbox_proxy_server.Text;
            Properties.Settings.Default.proxy_port = integerUpDown_proxy_port.Value.Value;
            Properties.Settings.Default.proxy_username = textbox_proxy_username.Text;
            Properties.Settings.Default.proxy_password = Helper.Crypto.EncryptString(passwordbox_proxy_password.SecurePassword);
            AppController.Current.toggle_proxy();
        }

        private void button_apply_Click(object sender, RoutedEventArgs e)
        {
            set_proxy();
        }

    }
}
