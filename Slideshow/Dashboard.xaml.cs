using NaturalSort.Extension;
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
        private string FileDirectory;
        private string[] Files;
        private int CurrentFile;

        public Dashboard(string path)
        {

            // get the file attributes for file or directory
            FileAttributes attr = File.GetAttributes(path);

            //detect whether its a directory or file
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                Console.WriteLine("It's a directory");
                Initialize(GetImageFiles(path)[0]);
            }
            else
            {
                Console.WriteLine("It's a file");
                Initialize(path);
            }
        }
        
        private void Initialize(string filePath)
        {
            InitializeComponent();
            FileDirectory = new FileInfo(filePath).DirectoryName; // Set Directory of where the file is located in
            Files = GetImageFiles(FileDirectory);
            CurrentFile = Array.IndexOf(Files, filePath); // Set CurrentFile to the int corresponding to the file in the folder
            UpdateImage(); // Update image to the CurrentFile path
            
            FileSystemWatcher watcher = new FileSystemWatcher()
            {
                Path = FileDirectory,
                EnableRaisingEvents = true
            };
            watcher.Created += new FileSystemEventHandler(OnUpdate);
            watcher.Deleted += new FileSystemEventHandler(OnUpdate);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            string file = Files[CurrentFile];
            Files = GetImageFiles(FileDirectory);
            // check to see if it's the current file being re-named
            if (file == e.OldFullPath)
            {
                // check to see if the current file is still there
                if (Files.Contains(e.FullPath))
                {
                    // Move pointer to the new file location (in the array)
                    CurrentFile = Array.IndexOf(Files, e.FullPath);
                }
            }
            PreventScopeMiss();
            UpdateImage();
        }

        private void OnUpdate(object source, FileSystemEventArgs e)
        {
            string file = Files[CurrentFile];
            Files = GetImageFiles(FileDirectory);
            // check to see if the current file is still there
            if (Files.Contains(file))
            {
                // Move pointer to the new file location (in the array)
                CurrentFile = Array.IndexOf(Files, file);
            }
            PreventScopeMiss();
            UpdateImage();
        }

        // Right now this just handles exscape or enter keys to make it a lot more keyboard friendly
        private void FormKeyDown(object sender, KeyEventArgs e)
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
                new Fullscreen(Files[CurrentFile]).Show();
            }
            if (e.Key == Key.Delete)
            {
                try
                {
                    File.Delete(Files[CurrentFile]);
                }
                catch (IOException)
                {

                }
            }
        }

        private void NextImage()
        {
            CurrentFile++;
            while (CurrentFile >= Files.Length)
            {
                CurrentFile -= Files.Length;
            }
            PreventScopeMiss();
            UpdateImage();
        }

        private void PrevImage()
        {
            CurrentFile--;
            while (CurrentFile < 0)
            {
                CurrentFile += Files.Length;
            }
            PreventScopeMiss();
            UpdateImage();
        }

        private void PreventScopeMiss()
        {
            while (CurrentFile >= Files.Length)
            {
                CurrentFile = Files.Length-1;
            }
        }

        private void UpdateImage()
        {
            Amount.Dispatcher.Invoke(() =>
            {
                ProgressFull.Width = new GridLength(CurrentFile, GridUnitType.Star);
                ProgressEmpty.Width = new GridLength(Files.Length - CurrentFile - 1, GridUnitType.Star);
                Amount.Content = (CurrentFile + 1) + " / " + Files.Length;
                try
                {
                    ErrorMessage.Content = null;
                    UniversalContenter.ClearImage(Content);
                    UniversalContenter.ChangeImage(Content, Files[CurrentFile]);
                }
                catch (Exception)
                {
                    // NotSupportedException || NullReferenceException
                    try
                    {
                        ErrorMessage.Content = new FileInfo(Files[CurrentFile]).Name + " is not supported";
                    }
                    catch
                    {
                        ErrorMessage.Content = "No files to display";
                    }
                }
                try
                {
                    Title = new FileInfo(Files[CurrentFile]).Name + " - Slideshow";
                }
                catch
                {
                    ErrorMessage.Content = "No files to display";
                }
            });
        }

        private string[] GetImageFiles(string path)
        {
            List<string> extensions = new List<string> { ".jpeg", ".jpg", ".gif", ".png" };
            IEnumerable<String> files = Directory.GetFiles(path).Where(s => extensions.Contains(Path.GetExtension(s)));
            string[] fileArray = files.ToArray();
            fileArray = fileArray.OrderBy(x => x, StringComparer.OrdinalIgnoreCase.WithNaturalSort()).ToArray();
            return fileArray;
            // return list.toArray().Length == 0 ? null : list.toArray();
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
