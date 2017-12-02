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
        private string[] Files;
        //private string FileDirectory;
        private int CurrentFile;

        public Dashboard(string filePath)
        {
            Initialize(filePath);
        }

        public Dashboard()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string[] files = GetImageFiles(path);
            Initialize(files[0]);
        }

        private void Initialize(string filePath)
        {
            InitializeComponent();
            UpdateFiles(new FileInfo(filePath).DirectoryName);
            CurrentFile = Array.IndexOf(Files, filePath);
            UpdateImage(filePath);
        }

        // Right now this just handles exscape or enter keys to make it a lot more keyboard friendly
        private void FormKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                NextImage();
            }
            if (e.Key == Key.Left)
            {
                PrevImage();
            }
            if (e.Key == Key.Escape)
            {
                Close();
            }
            if (e.Key == Key.Enter)
            {
                //StartView();
            }
        }

        private void NextImage()
        {
            // Incrementer focus
            CurrentFile++;
            if (CurrentFile >= Files.Length)
            {
                CurrentFile -= Files.Length;
            }
            UpdateImage(Files[CurrentFile]);
        }

        private void PrevImage()
        {
            // Incrementer focus
            CurrentFile--;
            if (CurrentFile < 0)
            {
                CurrentFile += Files.Length;
            }
            UpdateImage(Files[CurrentFile]);
        }

        private void UpdateImage(string filePath)
        {
            FileInfo currentFile = new FileInfo(filePath);
            string[] files = GetImageFiles(currentFile.DirectoryName);
            Amount.Content = (CurrentFile+1) + " / " + files.Length;
            try
            {
                ErrorMessage.Content = null;
                UniversalContenter.ChangeImage(Content, filePath);
            }
            catch (Exception)
            {
                // NotSupportedException || NullReferenceException
                ErrorMessage.Content = (currentFile.Name) + " is not supported";
            }
            this.Title = currentFile.Name + " - Slideshow";
        }

        private string[] GetImageFiles(string directory)
        {
            List<string> extensions = new List<string> { ".jpeg", ".jpg", ".gif", ".png" };
            IEnumerable<String> files = Directory.GetFiles(directory).Where(s => extensions.Contains(Path.GetExtension(s)));
            return files.ToArray();
        }

        private void UpdateFiles(string directory)
        {
            Files = GetImageFiles(directory);
        }

        /*private void StartView(object sender, RoutedEventArgs e)
        {
            StartView();
        }*/

        /*private void StartView()
        {
            ImageController imgControl = ImageController.Instance;
            
            try // to set the path for the folder
            {
                imgControl.FileEntries = Directory.GetFiles(Path.Text);
            }
            catch (DirectoryNotFoundException) // if the given folder doesn't exists
            {
                ErrorMessage.Content = "Could not find given path!";
                return;
            }
            
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
                imgControl.FileEntries = Shuffle(imgControl.FileEntries);
            }

            imgControl.ImageViews = imageViews;
            imgControl.Start();

            Close();
        }*/

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
