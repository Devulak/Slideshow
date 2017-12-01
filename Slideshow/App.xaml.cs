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
        // Only used for debugging so the reset doesn't happen every instance of the dashboard window
        public App()
        {
            // For debugging, reset the user input
            if (Debugger.IsAttached)
            {
                Slideshow.Properties.Settings.Default.Reset();
            }
        }
    }
}
