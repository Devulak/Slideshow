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
    /// Interaction logic for ImageView.xaml
    /// </summary>
    public partial class ImageView : Window
    {
        private ImageControllerOld imgControl;

        public ImageView()
        {
            InitializeComponent();
            imgControl = ImageControllerOld.Instance;
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Dashboard main = new Dashboard();
                main.Show();
                imgControl.Stop();
                foreach (ImageView Looper in imgControl.ImageViews)
                {
                    Looper.Close();
                }
            }
            if (e.Key == Key.Space)
            {
                if (imgControl.Run)
                {
                    imgControl.Stop();
                }
                else
                {
                    imgControl.Start();
                }
            }
            if (e.Key == Key.Right)
            {
                imgControl.NextImage();
            }
            if (e.Key == Key.Left)
            {
                imgControl.PrevImage();
            }
        }
    }
}
