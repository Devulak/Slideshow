using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;


namespace Slideshow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            // For debugging, reset the user input
            if (Debugger.IsAttached)
            {
                Slideshow.Properties.Settings.Default.Reset();
            }

            string[] args = e.Args;
            Dashboard dashboard = new Dashboard(args);
            dashboard.Show();
        }
    }
}
