using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Slideshow
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        public Dashboard(string[] args)
        {
            InitializeComponent();
            if (args.Length != 0)
            {
                FileInfo f = new FileInfo(args[0]);
                Startup(f.DirectoryName);
            }
            else
            {
                Startup();
            }
        }

        public Dashboard()
        {
            InitializeComponent();
            Startup();
        }

        private void Startup()
        {
            // If the initial path hasn't been set, do so
            if (Properties.Settings.Default.Path == "")
            {
                Properties.Settings.Default.Path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }
            Startup(Properties.Settings.Default.Path);
        }
        private void Startup(string path)
        {
            Path.Text = path;
            Time.Text = Properties.Settings.Default.Timer.ToString();
            Randomized.IsChecked = Properties.Settings.Default.Randomized;
        }

        private void StartView(object sender, RoutedEventArgs e)
        {
            ImageController imgControl = ImageController.Instance;
            
            try // to set the path for the folder
            {
                // TODO: fix this "hack"
                string[] directoryFiles = Directory.GetFiles(Path.Text);
                List<string> listOfFiles = new List<string>();
                foreach (string file in directoryFiles)
                {
                    FileInfo f = new FileInfo(file);
                    string[] acceptedFileTypes = { "jpg", "jpeg", "png", "gif" };
                    foreach (string fileType in acceptedFileTypes)
                    {
                        if (f.Extension == fileType)
                        {
                            listOfFiles.Add(file);
                            return;
                        }
                    }
                }
                imgControl.FileEntries = listOfFiles.ToArray();
            }
            catch (DirectoryNotFoundException) // if the given folder doesn't exists
            {
                ErrorMessage.Content = "Could not find given path!";
                return;
            }

            Properties.Settings.Default.Path = Path.Text;
            Properties.Settings.Default.Timer = Int32.Parse(Time.Text);
            Properties.Settings.Default.Randomized = (bool)Randomized.IsChecked;
            Properties.Settings.Default.Save();


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
            imgControl.Timer = Int32.Parse(Time.Text);

            if (Properties.Settings.Default.Randomized)
            {
                imgControl.FileEntries = Shuffle(Directory.GetFiles(Path.Text));
            }

            imgControl.ImageViews = imageViews;
            imgControl.Start();

            Close();
        }

        private void Grid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private string[] Shuffle(string[] files)
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
        }
    }
}
