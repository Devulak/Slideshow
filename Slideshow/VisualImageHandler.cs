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
                CurrentFileInfo = FileInfos.FirstOrDefault();
            }
            else // It's a file!
            {
                // File
                CurrentFileInfo = new FileInfo(path);
                DirectoryInfo = CurrentFileInfo.Directory;
                UpdateImagePathFiles(DirectoryInfo);
                CurrentFileInfo = FileInfos.Find(x => x.FullName == CurrentFileInfo.FullName); // Makes sure it points as the actual object in the list
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
            // check to see if it's the current file being re-named
            if (CurrentFileInfo.FullName == e.OldFullPath)
            {
                UpdateImagePathFiles(DirectoryInfo);
                // Move pointer to the new file location (in the array)
                CurrentFileInfo = FileInfos.Find(x => x.FullName == e.FullPath);
            }
            UpdateImage();
            Changed();
        }

        private void OnUpdate(object source, FileSystemEventArgs e)
        {
            UpdateImagePathFiles(DirectoryInfo);
            if (FileInfos.Count() > 0)
            {
                UpdateImagePathFiles(DirectoryInfo);

                // check to see if the current file is still there
                if (FileInfos.Find(x => x.FullName == e.FullPath) != null)
                {
                    // Move pointer to the new file location (in the array)
                    CurrentFileInfo = FileInfos.Find(x => x.FullName == e.FullPath);
                }
            }
            else
            {
                UpdateImagePathFiles(DirectoryInfo);
                CurrentFileInfo = FileInfos.FirstOrDefault();
            }
            UpdateImage();
            Changed();
        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            int index = FileInfos.FindIndex(x => x == CurrentFileInfo);
            if (FileInfos.Count() > 0)
            {
                FileInfo fi = FileInfos.Find(x => x.FullName == e.FullPath);
                FileInfos.Remove(fi);
            }
            else
            {
                UpdateImagePathFiles(DirectoryInfo);
                CurrentFileInfo = FileInfos.FirstOrDefault();
            }

            if (index >= FileInfos.Count())
            {
                index = FileInfos.Count() - 1;
            }
            if (FileInfos.Count() > 0)
            {
                CurrentFileInfo = FileInfos[index];
            }
            else
            {
                CurrentFileInfo = null;
            }

            UpdateImage();
            Changed();
        }

        public void NextImage()
        {
            if (FileInfos.Count() > 0) // Is there even an image?
            {
                int index = FileInfos.FindIndex(x => x == CurrentFileInfo);
                index++;
                while (index >= FileInfos.Count())
                {
                    index -= FileInfos.Count();
                }
                CurrentFileInfo = FileInfos[index];

                UpdateImage();
                Changed();
            }
        }

        public void PrevImage()
        {
            if (FileInfos.Count() > 0) // Is there even an image?
            {
                int index = FileInfos.FindIndex(x => x == CurrentFileInfo);
                index--;
                while (index < 0)
                {
                    index += FileInfos.Count();
                }
                CurrentFileInfo = FileInfos[index];

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

            if (FileInfos.Count() > 0 && CurrentFileInfo != null) // Is there even an image?
            {

                // Display the image
                Target.Dispatcher.Invoke(() =>
                {
                    while (true)
                    {
                        try
                        {
                            MemoryStream memory = new MemoryStream();
                            using (FileStream file = File.OpenRead(CurrentFileInfo.FullName))
                            {
                                file.CopyTo(memory);
                            }

                            memory.Seek(0, SeekOrigin.Begin);

                            var imageSource = new BitmapImage();
                            imageSource.BeginInit();
                            imageSource.StreamSource = memory;
                            imageSource.EndInit();

                            if(Path.GetExtension(CurrentFileInfo.Name).ToLower() == ".gif")
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
