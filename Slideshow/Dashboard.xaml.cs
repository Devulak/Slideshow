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
        private VisualImageHandler visualImageHandler;

        // selected image
        public Dashboard(string fullPath)
        {
            InitializeComponent();
            
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

            visualImageHandler = new VisualImageHandler(Content, fullPath);

            // add eventhandler
            visualImageHandler.Changed += new VisualEventHandler(OnChange);
            OnChange(); // Call to get initial values
        }

        private void OnChange()
        {
            Amount.Dispatcher.Invoke(() =>
            {
                if (visualImageHandler.FileInfos.Count() > 0)
                {
                    Title = visualImageHandler.CurrentFileInfo.Name + " - Fullscreen";
                    Amount.Content = (visualImageHandler.FileInfos.FindIndex(x => x == visualImageHandler.CurrentFileInfo) + 1) + " / " + visualImageHandler.FileInfos.Count();
                    ProgressFull.Width = new GridLength(visualImageHandler.FileInfos.FindIndex(x => x == visualImageHandler.CurrentFileInfo), GridUnitType.Star);
                    ProgressEmpty.Width = new GridLength(visualImageHandler.FileInfos.Count() - visualImageHandler.FileInfos.FindIndex(x => x == visualImageHandler.CurrentFileInfo) - 1, GridUnitType.Star);
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
                if (visualImageHandler.FileInfos.Count() > 0)
                {
                    new Fullscreen(visualImageHandler.CurrentFileInfo.FullName).Show();
                }
                else
                {
                    new Fullscreen(visualImageHandler.DirectoryInfo.FullName).Show();
                }
                Close();
            }
            if (e.Key == Key.Delete)
            {
                try
                {
                    File.Delete(visualImageHandler.CurrentFileInfo.FullName);
                }
                catch
                {

                }
            }
        }

        private void NextImage(object sender, RoutedEventArgs e)
        {
            visualImageHandler.NextImage();
        }

        private void PrevImage(object sender, RoutedEventArgs e)
        {
            visualImageHandler.PrevImage();
        }

        private void OpenAbout(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.Owner = this;
            about.ShowDialog();
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.Owner = this;
            settings.ShowDialog();
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

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Save preferences


            base.OnClosing(e);
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

        // Shuffle a string array
        // TODO: change it to List<string>
        /*private string[] Shuffle(string[] files)
        {
            Random rnd = new Random();

            for (int i = 0; i < files.Length; i++)
            {
                int ticket = rnd.Next(files.Length);
                string fileHolder = files[i]; // Hold the values in [i] before removing it
                files[i] = files[ticket]; // Replace [i] with the lottery [ticket]
                files[ticket] = fileHolder; // Replace [ticket] with the fileHolder which held the [i] value
            }
            return files;
        }*/
    }
}
