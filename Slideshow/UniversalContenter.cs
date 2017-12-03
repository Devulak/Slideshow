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
            MemoryStream memory = new MemoryStream();
            using (FileStream file = File.OpenRead(filePath))
            {
                file.CopyTo(memory);
            }

            memory.Seek(0, SeekOrigin.Begin);

            var imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.StreamSource = memory;
            imageSource.EndInit();
            target.Dispatcher.Invoke(() =>
            {
                ImageBehavior.SetAnimatedSource(target, imageSource);
            });
        }

        static public void ClearImage(Image target)
        {
            target.Dispatcher.Invoke(() =>
            {
                ImageBehavior.SetAnimatedSource(target, null);
            });
        }
    }
}
