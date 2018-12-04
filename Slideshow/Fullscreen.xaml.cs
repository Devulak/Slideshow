using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using System;
using System.ComponentModel;

namespace Slideshow
{
    /// <summary>
    /// Interaction logic for Fullscreen.xaml
    /// </summary>
    public partial class Fullscreen : Window
    {
        private VisualImageHandler ImageHandler;
        private Thread SlideshowThread;
        private bool RunSlideshow;

        // selected image
        public Fullscreen(string fullPath)
        {
            InitializeComponent();
            ImageHandler = new VisualImageHandler(Content, fullPath);
            Initialize();
        }

        public Fullscreen(VisualImageHandler visualImageHandler)
        {
            InitializeComponent();
            ImageHandler = visualImageHandler;
            ImageHandler.Target = Content;
            ImageHandler.UpdateImage();
            Initialize();
        }

        private void Initialize()
        {
            ImageHandler.Changed += new VisualEventHandler(OnChange); // add eventhandler
            OnChange(); // Call to get initial values

            // Slideshow thread
            RunSlideshow = false;
        }

        private void OnChange()
        {
            Amount.Dispatcher.Invoke(() =>
            {
                if (ImageHandler.FileInfos.Count() > 0)
                {
                    Title = ImageHandler.CurrentFileInfo.Name + " - Fullscreen";
                    Amount.Content = ImageHandler.FileInfos.FindIndex(x => x == ImageHandler.CurrentFileInfo) + 1 + " / " + ImageHandler.FileInfos.Count();
                    ProgressFull.Width = new GridLength(ImageHandler.FileInfos.FindIndex(x => x == ImageHandler.CurrentFileInfo), GridUnitType.Star);
                    ProgressEmpty.Width = new GridLength(ImageHandler.FileInfos.Count() - ImageHandler.FileInfos.FindIndex(x => x == ImageHandler.CurrentFileInfo) - 1, GridUnitType.Star);
                }
                else
                {
                    Title = "Fullscreen";
                    ErrorMessage.Content = "No files to show";
                    Amount.Content = "0 / 0";
                    ProgressFull.Width = new GridLength(0, GridUnitType.Star);
                    ProgressEmpty.Width = new GridLength(1, GridUnitType.Star);
                }
            });
        }

        private void Shortcuts(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    ImageHandler.NextImage();
                    break;

                case Key.Left:
                    ImageHandler.PrevImage();
                    break;

                case Key.Escape:
                    new Dashboard(ImageHandler).Show();
                    Close();
                    break;

                case Key.Space:
                    ToggleSlideshow();
                    break;
            }
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            RunSlideshow = false;
        }

        private void ToggleSlideshow()
        {
            if (RunSlideshow) // Is the slideshow running?
            {
                RunSlideshow = false; // Stop the slideshow and let the thread complete
            }
            else
            {
                RunSlideshow = true; // Run the slideshow
                if (SlideshowThread == null || !SlideshowThread.IsAlive) // Is the slideshow thread existense and dead?
                {
                    SlideshowThread = new Thread(ThreadWork) // Make a new one!
                    {
                        IsBackground = true
                    };
                    SlideshowThread.Start();
                }
            }
        }

        private void ThreadWork()
        {
            Thread.Sleep(Properties.Settings.Default.SlideshowDelay);
            if(RunSlideshow)
            {
                ImageHandler.NextImage();
                ThreadWork();
            }
        }
    }
}
