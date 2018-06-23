using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;

namespace Slideshow
{
    class ImageManipulation
    {
        private Image Target;
        private FrameworkElement Parent;

        public ImageManipulation(Image image)
        {
            Target = image;
            Parent = (FrameworkElement)Target.Parent; // Bold assumption m8, but we'll go with it for now


            Target.Stretch = Stretch.None;
            Target.SourceUpdated += new EventHandler<DataTransferEventArgs>(Content_SourceUpdated);
            Target.TargetUpdated += new EventHandler<DataTransferEventArgs>(Content_SourceUpdated);
            Target.SizeChanged += new SizeChangedEventHandler(Nani);
            Parent.SizeChanged += new SizeChangedEventHandler(Nani);
        }

        private void Content_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            Console.WriteLine("WOA");
        }

        private void Nani(object sender, SizeChangedEventArgs e)
        {
            if(Target.Source != null)
            {
                Target.Stretch = Stretch.Uniform;

                Console.WriteLine("SIZE CHANGED");
                Console.WriteLine(Parent.ActualWidth);
                Console.WriteLine(Parent.ActualHeight);
                if (Target.Source.Width <= Parent.ActualWidth && Target.Source.Height <= Parent.ActualHeight) // Is the size within' the borders?
                {
                    Target.Stretch = Stretch.None;
                }
            }
        }
    }
}
