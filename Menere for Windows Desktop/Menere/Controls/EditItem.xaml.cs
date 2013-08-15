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
    /// Interaction logic for EditItem.xaml
    /// </summary>
    public partial class EditItem : UserControl
    {
        public EditItem()
        {
            InitializeComponent();
        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            
            IItem item = this.DataContext as IItem;
            if (item != null)
            {
                if (item.is_read != checkbox_isRead.IsChecked.Value)
                {
                    if (checkbox_isRead.IsChecked.Value)
                    {
                        item.mark_read();
                    }
                    else
                    {
                        item.mark_unread();
                    }
                }

                if (item.is_saved != checkbox_isSaved.IsChecked.Value)
                {
                    if (checkbox_isSaved.IsChecked.Value)
                    {
                        item.mark_saved();
                    }
                    else
                    {
                        item.mark_unsaved();
                    }
                }

                if (item.tag_string != textbox_tags.Text)
                {
                    item.save_tags(textbox_tags.Text);
                }
            }
        }
    }
}
