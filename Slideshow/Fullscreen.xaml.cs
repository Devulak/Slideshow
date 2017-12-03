using System;
using System.Collections.Generic;
using System.IO;
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
using WpfAnimatedGif;

namespace Slideshow
{
    /// <summary>
    /// Interaction logic for Fullscreen.xaml
    /// </summary>
    public partial class Fullscreen : Window
    {
        private VisualImageHandler visualImageHandler;

        // selected image
        public Fullscreen(string fullPath)
        {
            InitializeComponent();
            
            visualImageHandler = new VisualImageHandler(Content, fullPath);

            // add eventhandler
            visualImageHandler.Changed += new VisualEventHandler(OnChange);
            OnChange(); // Call to get initial values
        }

        private void OnChange()
        {
            Amount.Dispatcher.Invoke(() =>
            {
                Amount.Content = (visualImageHandler.CurrentFile + 1) + " / " + visualImageHandler.Files.Length;
                ProgressFull.Width = new GridLength(visualImageHandler.CurrentFile, GridUnitType.Star);
                ProgressEmpty.Width = new GridLength(visualImageHandler.Files.Length - visualImageHandler.CurrentFile - 1, GridUnitType.Star);
            });
        }

        private void Shortcuts(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                visualImageHandler.NextImage();
            }
            if (e.Key == Key.Left)
            {
                visualImageHandler.PrevImage();
            }
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
