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

namespace Slideshow
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            Randomize.IsChecked = Properties.Settings.Default.Randomized;
            IncludeSub.IsChecked = Properties.Settings.Default.Hierarchy;
        }

        private void Shortcuts(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void RandomizeEnable(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Randomized = true;
            Properties.Settings.Default.Save();
        }

        private void RandomizeDisable(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Randomized = false;
            Properties.Settings.Default.Save();
        }

        private void IncludeSubEnable(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Hierarchy = true;
            Properties.Settings.Default.Save();
        }

        private void IncludeSubDisable(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Hierarchy = false;
            Properties.Settings.Default.Save();
        }
    }
}
