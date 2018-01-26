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

    public class VisualImageHandler
    {
        private static Random rng = new Random();
        private FileSystemWatcher Watcher;
        private int Seed;
        public DirectoryInfo DirectoryInfo { get; private set; }
        public List<FileInfo> FileInfos { get; private set; }
        public FileInfo CurrentFileInfo { get; private set; }
        public bool IncludeSubdir { get; private set; }
        public bool Randomize { get; private set; }
        public Image Target;

        public event VisualEventHandler Changed;

        public VisualImageHandler(Image target, string path)
        {
            Target = target;

            Randomize = false;
            IncludeSubdir = false;
            FileInfos = new List<FileInfo>();

            // get the file attributes for file or directory
            FileAttributes attr = File.GetAttributes(path);

            //detect whether its a directory or file
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                // Directory
                DirectoryInfo = new DirectoryInfo(path);
                UpdateImagePathFiles();
                CurrentFileInfo = FileInfos.FirstOrDefault();
            }
            else // It's a file!
            {
                // File
                CurrentFileInfo = new FileInfo(path);
                DirectoryInfo = CurrentFileInfo.Directory;
                UpdateImagePathFiles();
                CurrentFileInfo = FileInfos.Find(x => x.FullName == CurrentFileInfo.FullName); // Makes sure it points as the actual object in the list
            }
            UpdateImage(); // Update image to the CurrentFile path

            Watcher = new FileSystemWatcher()
            {
                Path = DirectoryInfo.FullName,
                EnableRaisingEvents = true
            };
            Watcher.Deleted += new FileSystemEventHandler(OnDeleted);
            // watcher.Renamed += new RenamedEventHandler(OnRenamed);
            // watcher.Created += new FileSystemEventHandler(OnCreated);
        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            int index = FileInfos.FindIndex(x => x == CurrentFileInfo); // Get the index of the current file being displayed (-1 if not found [no files])

            // Try and find the item in the list, if it's there, remote it, if not, continue
            FileInfo fi = FileInfos.Find(x => x.FullName == e.FullPath);
            FileInfos.Remove(fi);

            if (!FileInfos.Contains(CurrentFileInfo)) // is the selected file removed? If not, just continue the day
            {
                if (index >= FileInfos.Count()) // is the index above what's suppose to be?
                {
                    index = FileInfos.Count() - 1; // Move it down a Markus Persson (Notch)
                }

                if (FileInfos.Count() > 0) // Is there still items in the list?
                {
                    CurrentFileInfo = FileInfos[index]; // Select that file
                }
                else // No items in the list
                {
                    CurrentFileInfo = null; // Select nothing for the current file info
                }

                UpdateImage(); // update only if it's a new file
            }

            Changed(); // Something changed! (Removed a file!)
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            FileInfo fi = FileInfos.Find(x => x.FullName == e.OldFullPath); // Get the file being renamed

            if (fi != null) // Is the file being enamed is part of the list?
            {

            }

            // check to see if it's the current file being re-named
            if (CurrentFileInfo.FullName == e.OldFullPath)
            {
                UpdateImagePathFiles();
                // Move pointer to the new file location (in the array)
                CurrentFileInfo = FileInfos.Find(x => x.FullName == e.FullPath);
            }
            UpdateImage();
            Changed(); // call event changed
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            UpdateImagePathFiles();
            if (FileInfos.Count() > 0)
            {
                UpdateImagePathFiles();

                // check to see if the current file is still there
                if (FileInfos.Find(x => x.FullName == e.FullPath) != null)
                {
                    // Move pointer to the new file location (in the array)
                    CurrentFileInfo = FileInfos.Find(x => x.FullName == e.FullPath);
                }
            }
            else
            {
                UpdateImagePathFiles();
                CurrentFileInfo = FileInfos.FirstOrDefault();
            }
            UpdateImage();
            Changed();
        }

        public void IncludeSubdirectories(bool answer)
        {
            Watcher.IncludeSubdirectories = answer;
            if (IncludeSubdir != answer)
            {
                IncludeSubdir = answer;
                UpdateImagePathFiles();
                UpdateImage();
                Changed();
            }
        }

        public void ShuffleDirectory(bool answer)
        {
            ShuffleDirectory(answer, rng.Next());
        }

        public void ShuffleDirectory(bool answer, int seed)
        {
            if (Randomize != answer)
            {
                Seed = rng.Next();
                Randomize = answer;
                SortFileInfos();
                Changed();
            }
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

        public void UpdateImage()
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

        private void UpdateImagePathFiles()
        {
            List<FileInfo> fileInfos;
            if (IncludeSubdir)
            {
                fileInfos = DirectoryInfo.GetFiles("*", SearchOption.AllDirectories).ToList();
            }
            else
            {
                fileInfos = DirectoryInfo.GetFiles().ToList();
            }

            // Remove anything not an image
            List<string> extensions = new List<string>() { ".jpeg", ".jpg", ".gif", ".png" };
            fileInfos = fileInfos.Where(s => extensions.Contains(Path.GetExtension(s.Name).ToLower())).ToList(); // have a complete list of all the images

            // Set the current file info to the corret one in the new list
            if(CurrentFileInfo != null && fileInfos.Exists(x => x.FullName == CurrentFileInfo.FullName))
            {
                CurrentFileInfo = fileInfos.Find(x => x.FullName == CurrentFileInfo.FullName);
            }
            else // no file was actually found, reset!
            {
                CurrentFileInfo = fileInfos.FirstOrDefault();
            }

            // Result
            FileInfos = fileInfos;

            SortFileInfos();
        }

        private void SortFileInfos()
        {
            // Sort current List
            if (Randomize)
            {
                FileInfos.Shuffle(Seed);
            }
            else
            {
                // Sort it naturally
                NaturalFileInfoNameComparer comparer = new NaturalFileInfoNameComparer();
                FileInfos.Sort(comparer);
                //fileInfos.Sort(SafeNativeMethods.StrCmpLogicalW);
            }
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
                return SafeNativeMethods.StrCmpLogicalW(a.FullName, b.FullName);
            }
        }
    }
}
