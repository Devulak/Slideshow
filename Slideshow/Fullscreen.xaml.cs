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

namespace Slideshow
{
    /// <summary>
    /// Interaction logic for Fullscreen.xaml
    /// </summary>
    public partial class Fullscreen : Window
    {
        private bool IsLimited = false;

        // selected image
        public Fullscreen(FileInfo fileInfo)
        {
            InitializeComponent();
        }

        // path or limited images
        public Fullscreen(FileInfo[] fileInfo, bool isLimited)
        {
            InitializeComponent();
            IsLimited = isLimited;
        }
        
        private  Fullscreen()
        {
        }
    }
}
