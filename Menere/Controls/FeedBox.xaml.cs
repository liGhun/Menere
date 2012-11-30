using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace Menere.Controls
{
    /// <summary>
    /// Interaction logic for FeedBox.xaml
    /// </summary>
    public partial class FeedBox : UserControl
    {
        public FeedBox()
        {
            InitializeComponent();
        }

        private void Item_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                Storyboard story;
                story = (Storyboard)this.FindResource("FadeIn");
                if (story != null)
                {
                    story.Begin();
                }
                else
                {
                    this.wrapPanelAvatarOverlay.Opacity = 0.75;
                }
            }
            catch
            {
                try
                {
                    this.wrapPanelAvatarOverlay.Opacity = 0.75;
                }
                catch
                {
                    //AppController.Current.Logger.writeToLogfile("wrapPanelAvatar failed");
                }
            }
        }

        private void Item_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                Storyboard story;
                story = (Storyboard)this.FindResource("FadeAway");
                if (story != null)
                {
                    story.Begin();
                }
                else
                {
                    try
                    {
                        this.wrapPanelAvatarOverlay.Opacity = 0;
                    }
                    catch
                    {
                      //  AppController.Current.Logger.writeToLogfile("wrapPanelAvatar failed");
                    }
                }
            }
            catch
            {
                this.wrapPanelAvatarOverlay.Opacity = 0;
            }
        }

    }
}
