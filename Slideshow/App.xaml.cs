using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
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

            // 
            if (e.Args != null && e.Args.Length != 0)
            {
                Dashboard dashboard = new Dashboard(e.Args[0]);
                dashboard.Show();
            }
            else
            {
                Dashboard dashboard = new Dashboard();
                dashboard.Show();
            }
        }
    }
}
