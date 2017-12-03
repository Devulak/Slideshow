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
    class VisualImageHandler
    {
        private string FileDirectory;
        private string[] Files;
        private int CurrentFile;
        private Image Target;

        public VisualImageHandler(Image target, string path)
        {
            Target = target;

            // get the file attributes for file or directory
            FileAttributes attr = File.GetAttributes(path);

            //detect whether its a directory or file
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                Initialize(GetImageFiles(path)[0]);
            }
            else
            {
                Initialize(path);
            }
        }

        private void Initialize(string fullPath)
        {
            FileDirectory = new FileInfo(fullPath).DirectoryName; // Set Directory of where the file is located in
            Files = GetImageFiles(FileDirectory);
            CurrentFile = Array.IndexOf(Files, fullPath); // Set CurrentFile to the int corresponding to the file in the folder
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

        public void NextImage()
        {
            CurrentFile++;
            while (CurrentFile >= Files.Length)
            {
                CurrentFile -= Files.Length;
            }
            PreventScopeMiss();
            UpdateImage();
        }

        public void PrevImage()
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
                CurrentFile = Files.Length - 1;
            }
        }

        private void UpdateImage()
        {
            Target.Dispatcher.Invoke(() =>
            {
                try
                {
                    Target.Dispatcher.Invoke(() =>
                    {
                        ImageBehavior.SetAnimatedSource(Target, null);
                    });

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

                    Target.Dispatcher.Invoke(() =>
                    {
                        ImageBehavior.SetAnimatedSource(Target, imageSource);
                    });
                }
                catch (Exception)
                {
                    // NotSupportedException || NullReferenceException
                    try
                    {
                        Console.WriteLine(new FileInfo(Files[CurrentFile]).Name + " is not supported");
                    }
                    catch
                    {
                        Console.WriteLine("No files to display");
                    }
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
    }
}
