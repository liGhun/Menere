﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Menere
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void Application_Startup(object sender, StartupEventArgs e)
        {
            AppController.Start();
            /* try
            {
                AppController.Start();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.StackTrace, exp.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            } */
        }
    }
}
