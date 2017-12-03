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

            // TODO: make sure the class can't get initialized without either of the following:
            // file
            // a path

            // Q: What if the files are filtered out? (practically showing 0 files in the dashboard)
            // A: Right now it should work as a "perfect world", afterwards, the VisualImageHandler should take care of wrongdoings

            string path = (e.Args != null && e.Args.Length != 0) ? e.Args[0] : Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            Dashboard dashboard = new Dashboard(path);
            dashboard.Show();
        }
    }
}
