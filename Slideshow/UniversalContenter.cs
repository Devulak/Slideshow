using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace Slideshow
{
    class UniversalContenter
    {
        static public void ChangeImage(Image target, string filePath)
        {
            target.Dispatcher.Invoke(() =>
            {
                ImageBehavior.SetAnimatedSource(target, null);
                var image = new BitmapImage(new Uri(filePath));
                ImageBehavior.SetAnimatedSource(target, image);
            });
        }
    }
}
