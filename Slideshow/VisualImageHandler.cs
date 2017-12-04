using NaturalSort.Extension;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace Slideshow
{
    public delegate void VisualEventHandler();

    class VisualImageHandler
    {
        public string FileDirectory { get; private set; }
        public string[] Files { get; private set; }
        public int CurrentFile { get; private set; }
        private Image Target;

        public event VisualEventHandler Changed;

        public VisualImageHandler(Image target, string path)
        {
            Target = target;

            // get the file attributes for file or directory
            FileAttributes attr = File.GetAttributes(path);

            //detect whether its a directory or file
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                // Directory
                FileDirectory = path;
                Files = GetImageFiles(FileDirectory);
                CurrentFile = 0;
            }
            else
            {
                // File
                FileDirectory = new FileInfo(path).DirectoryName;
                Files = GetImageFiles(FileDirectory);
                CurrentFile = Array.IndexOf(Files, path);
            }
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
            // check to see if it's the current file being re-named
            if (file == e.OldFullPath)
            {
                Files = GetImageFiles(FileDirectory);
                // Move pointer to the new file location (in the array)
                CurrentFile = Array.IndexOf(Files, e.FullPath);
            }
            UpdateImage();
            Changed();
        }

        private void OnUpdate(object source, FileSystemEventArgs e)
        {
            Console.WriteLine(e.FullPath);
            if (GetImageFiles(FileDirectory).Length > 0 && Files.Length > 0)
            {
                string file = Files[CurrentFile];
                Files = GetImageFiles(FileDirectory);
                // check to see if the current file is still there
                if (Files.Contains(file))
                {
                    // Move pointer to the new file location (in the array)
                    CurrentFile = Array.IndexOf(Files, file);
                }
            }
            else
            {
                Files = GetImageFiles(FileDirectory);
                CurrentFile = 0;
            }
            UpdateImage();
            Changed();
        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            Console.WriteLine(e.FullPath);
            if (GetImageFiles(FileDirectory).Length > 0 && Files.Length > 0)
            {
                string file = Files[CurrentFile];
                Files = GetImageFiles(FileDirectory);
                // check to see if the current file is still there
                if (Files.Contains(file))
                {
                    // Move pointer to the new file location (in the array)
                    CurrentFile = Array.IndexOf(Files, file);
                }
            }
            else
            {
                Files = GetImageFiles(FileDirectory);
                CurrentFile = 0;
            }
            UpdateImage();
            Changed();
        }

        public void NextImage()
        {
            if (Files.Length > 0) // Is there even an image?
            {
                CurrentFile++;
                while (CurrentFile >= Files.Length)
                {
                    CurrentFile -= Files.Length;
                }
                UpdateImage();
                Changed();
            }
        }

        public void PrevImage()
        {
            if (Files.Length > 0) // Is there even an image?
            {
                CurrentFile--;
                while (CurrentFile < 0)
                {
                    CurrentFile += Files.Length;
                }
                UpdateImage();
                Changed();
            }
        }

        private void UpdateImage()
        {
            // Clear canvas
            Target.Dispatcher.Invoke(() =>
            {
                ImageBehavior.SetAnimatedSource(Target, null);
            });

            if (Files.Length > 0) // Is there even an image?
            {
                // Make sure to point at a picture, no matter what the CurrentFile is 
                // TODO: fix if two or more files are being deleted
                if (CurrentFile >= Files.Length)
                {
                    CurrentFile = Files.Length - 1;
                }

                // vs

                CurrentFile = (CurrentFile >= Files.Length) ? (Files.Length - 1) : CurrentFile;

                // Display the image
                Target.Dispatcher.Invoke(() =>
                {
                    while (true)
                    {
                        try
                        {
                            MemoryStream memory = new MemoryStream();
                            using (FileStream file = File.OpenRead(Files[CurrentFile]))
                            {
                                file.CopyTo(memory);
                            }

                            memory.Seek(0, SeekOrigin.Begin);

                            var imageSource = new BitmapImage();
                            imageSource.BeginInit();
                            imageSource.StreamSource = memory;
                            imageSource.EndInit();

                            ImageBehavior.SetAnimatedSource(Target, imageSource);
                            break;
                        }
                        catch
                        {

                        }
                    }
                });
            }
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
    }
}
