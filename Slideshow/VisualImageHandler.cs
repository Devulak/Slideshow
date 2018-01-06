using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
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
        public string[] Files { get; private set; }
        public int CurrentFile { get; private set; }
        public DirectoryInfo DirectoryInfo { get; private set; }
        public List<FileInfo> FileInfos { get; private set; }
        public FileInfo CurrentFileInfo { get; private set; }
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
                DirectoryInfo = new DirectoryInfo(path);
                UpdateImagePathFiles(DirectoryInfo);
                CurrentFile = 0;
            }
            else
            {
                // File
                DirectoryInfo = new FileInfo(path).Directory;
                UpdateImagePathFiles(DirectoryInfo);
                CurrentFile = Array.IndexOf(Files, path);
            }
            UpdateImage(); // Update image to the CurrentFile path

            FileSystemWatcher watcher = new FileSystemWatcher()
            {
                Path = DirectoryInfo.FullName,
                EnableRaisingEvents = true
            };
            watcher.Created += new FileSystemEventHandler(OnUpdate);
            watcher.Deleted += new FileSystemEventHandler(OnDeleted);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            string file = Files[CurrentFile];
            // check to see if it's the current file being re-named
            if (file == e.OldFullPath)
            {
                UpdateImagePathFiles(DirectoryInfo);
                // Move pointer to the new file location (in the array)
                CurrentFile = Array.IndexOf(Files, e.FullPath);
            }
            UpdateImage();
            Changed();
        }

        private void OnUpdate(object source, FileSystemEventArgs e)
        {
            Console.WriteLine(e.FullPath);
            UpdateImagePathFiles(DirectoryInfo);
            if (Files.Length > 0 && Files.Length > 0)
            {
                string file = Files[CurrentFile];

                UpdateImagePathFiles(DirectoryInfo);

                // check to see if the current file is still there
                if (Files.Contains(file))
                {
                    // Move pointer to the new file location (in the array)
                    CurrentFile = Array.IndexOf(Files, file);
                }
            }
            else
            {
                UpdateImagePathFiles(DirectoryInfo);
                CurrentFile = 0;
            }
            UpdateImage();
            Changed();
        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            if (Files.Length > 0)
            {
                List<string> list = new List<string>(Files);
                list.Remove(e.FullPath);
                Files = list.ToArray();
            }
            else
            {
                UpdateImagePathFiles(DirectoryInfo);
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
                Target.Source = null;
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

                            if(Path.GetExtension(Files[CurrentFile]).ToLower() == ".gif")
                            {
                                ImageBehavior.SetAnimatedSource(Target, imageSource);
                            }
                            else
                            {
                                Target.Source = imageSource;
                            }
                            
                            break;
                        }
                        catch
                        {
                            break;
                        }
                    }
                });
            }
        }

        private void UpdateImagePathFiles(DirectoryInfo directoryInfo)
        {
            List<FileInfo> fileInfos = directoryInfo.GetFiles().ToList();

            // Remove anything not an image
            List<string> extensions = new List<string>() { ".jpeg", ".jpg", ".gif", ".png" };
            fileInfos = fileInfos.Where(s => extensions.Contains(Path.GetExtension(s.Name).ToLower())).ToList();

            // Sort it naturally
            // Hack, very poor performance, but it works for now.. 
            NaturalFileInfoNameComparer comparer = new NaturalFileInfoNameComparer();
            fileInfos.Sort(comparer);
            //fileInfos.Sort(SafeNativeMethods.StrCmpLogicalW);

            // Result
            FileInfos = fileInfos;

            // Result Old
            List<string> fileList = new List<string>();
            foreach(FileInfo fileInfo in fileInfos)
            {
                fileList.Add(fileInfo.FullName);
            }
            string[] fileArray = fileList.ToArray();
            Files = fileArray;
        }

        [SuppressUnmanagedCodeSecurity]
        internal static class SafeNativeMethods
        {
            [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
            public static extern int StrCmpLogicalW(string psz1, string psz2);
        }

        public sealed class NaturalStringComparer : IComparer<string>
        {
            public int Compare(string a, string b)
            {
                return SafeNativeMethods.StrCmpLogicalW(a, b);
            }
        }

        public sealed class NaturalFileInfoNameComparer : IComparer<FileInfo>
        {
            public int Compare(FileInfo a, FileInfo b)
            {
                return SafeNativeMethods.StrCmpLogicalW(a.Name, b.Name);
            }
        }
    }
}
