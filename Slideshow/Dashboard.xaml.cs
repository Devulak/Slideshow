using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Slideshow
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        private VisualImageHandler VisualImageHandler;
        private ImageManipulation imageManipulation;

        // selected image
        public Dashboard(string fullPath)
        {
            InitializeComponent();
            VisualImageHandler = new VisualImageHandler(Content, fullPath);
            Initialize();
        }

        // selected image
        public Dashboard(VisualImageHandler visualImageHandler)
        {
            InitializeComponent();
            VisualImageHandler = visualImageHandler;
            VisualImageHandler.Target = Content;
            VisualImageHandler.UpdateImage();
            Initialize();
        }

        private void Initialize()
        {
            if (Properties.Settings.Default.WindowCustomized)
            {
                Width = Properties.Settings.Default.WindowWidth;
                Height = Properties.Settings.Default.WindowHeight;
                Left = Properties.Settings.Default.WindowLeft;
                Top = Properties.Settings.Default.WindowTop;

                // Check for maximized after the movement
                if (Properties.Settings.Default.WindowMaximized)
                {
                    WindowState = WindowState.Maximized;
                }
            }


            // add eventhandler
            VisualImageHandler.Changed += new VisualEventHandler(OnChange);
            OnChange(); // Call to get initial values

            // set the properties
            VisualImageHandler.ShuffleDirectory(Properties.Settings.Default.Randomized);
            VisualImageHandler.IncludeSubdirectories(Properties.Settings.Default.Hierarchy);

            Randomize.IsChecked = Properties.Settings.Default.Randomized;
            IncludeSub.IsChecked = Properties.Settings.Default.Hierarchy;

            // Image manipulation, aka zoom and size perfection
            imageManipulation = new ImageManipulation(Content);
        }

        private void OnChange()
        {
            Amount.Dispatcher.Invoke(() =>
            {
                if (VisualImageHandler.FileInfos.Count() > 0)
                {
                    Title = VisualImageHandler.CurrentFileInfo.Name + " - Fullscreen";
                    Amount.Content = (VisualImageHandler.FileInfos.FindIndex(x => x == VisualImageHandler.CurrentFileInfo) + 1) + " / " + VisualImageHandler.FileInfos.Count();
                    ProgressFull.Width = new GridLength(VisualImageHandler.FileInfos.FindIndex(x => x == VisualImageHandler.CurrentFileInfo), GridUnitType.Star);
                    ProgressEmpty.Width = new GridLength(VisualImageHandler.FileInfos.Count() - VisualImageHandler.FileInfos.FindIndex(x => x == VisualImageHandler.CurrentFileInfo) - 1, GridUnitType.Star);
                }
                else
                {
                    Title = "Slideshow";
                    ErrorMessage.Content = "No files to show";
                    Amount.Content = "0 / 0";
                    ProgressFull.Width = new GridLength(0, GridUnitType.Star);
                    ProgressEmpty.Width = new GridLength(1, GridUnitType.Star);
                }
            });
        }

        private void Shortcuts(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                NextImage(sender, e);
            }
            if (e.Key == Key.Left)
            {
                PrevImage(sender, e);
            }
            if (e.Key == Key.Escape)
            {
                Close();
            }
            if (e.Key == Key.Enter)
            {
                new Fullscreen(VisualImageHandler).Show();
                Close();
            }
            if (e.Key == Key.Delete)
            {
                try
                {
                    File.Delete(VisualImageHandler.CurrentFileInfo.FullName);
                }
                catch
                {

                }
            }
        }

        private void NextImage(object sender, RoutedEventArgs e)
        {
            VisualImageHandler.NextImage();
        }

        private void PrevImage(object sender, RoutedEventArgs e)
        {
            VisualImageHandler.PrevImage();
        }

        private void OpenAbout(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.Owner = this;
            about.ShowDialog();
        }

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Fullscreen checkup
            if (WindowState == WindowState.Maximized)
            {
                Properties.Settings.Default.WindowMaximized = true;
            }
            else
            {
                Properties.Settings.Default.WindowMaximized = false;
            }

            // Size checkup
            if (Properties.Settings.Default.WindowMaximized == false)
            {
                Properties.Settings.Default.WindowWidth = Width;
                Properties.Settings.Default.WindowHeight = Height;
            }

            Properties.Settings.Default.WindowCustomized = true;
            Properties.Settings.Default.Save();
        }

        private void WindowLocationChanged(object sender, EventArgs e)
        {
            // Position checkup
            if (Properties.Settings.Default.WindowMaximized == false)
            {
                Properties.Settings.Default.WindowLeft = Left;
                Properties.Settings.Default.WindowTop = Top;
            }

            Properties.Settings.Default.WindowCustomized = true;
            Properties.Settings.Default.Save();
        }

        private void RandomizeEnable(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Randomized = true;
            Properties.Settings.Default.Save();
            VisualImageHandler.ShuffleDirectory(Properties.Settings.Default.Randomized);
        }

        private void RandomizeDisable(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Randomized = false;
            Properties.Settings.Default.Save();
            VisualImageHandler.ShuffleDirectory(Properties.Settings.Default.Randomized);
        }

        private void IncludeSubEnable(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Hierarchy = true;
            Properties.Settings.Default.Save();
            VisualImageHandler.IncludeSubdirectories(Properties.Settings.Default.Hierarchy);
        }

        private void IncludeSubDisable(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Hierarchy = false;
            Properties.Settings.Default.Save();
            VisualImageHandler.IncludeSubdirectories(Properties.Settings.Default.Hierarchy);
        }

        private void StartView()
        {
            /*ImageController imgControl = ImageController.Instance;
            imgControl.FileEntries = Files;

            /*Properties.Settings.Default.Timer = Int32.Parse(Time.Text);
            Properties.Settings.Default.Randomized = (bool)Randomized.IsChecked;
            Properties.Settings.Default.Save();*//*


            Screen[] screens = Screen.AllScreens;

            screens = screens.OrderBy(c => c.WorkingArea.Left).ToArray();

            List<ImageView> imageViews = new List<ImageView>();

            foreach (Screen screen in screens)
            {
                ImageView imageView = new ImageView();
                imageViews.Add(imageView);
                imageView.Show();
                imageView.Top = screen.WorkingArea.Top;
                imageView.Left = screen.WorkingArea.Left;
                imageView.Width = screen.WorkingArea.Width;
                imageView.Height = screen.WorkingArea.Height;
                imageView.WindowState = WindowState.Maximized;
            }

            imgControl.Reset();
            imgControl.Timer = Properties.Settings.Default.Timer;

            imgControl.ImageViews = imageViews;
            imgControl.Start();

            Close();*/
        }
    }
}
