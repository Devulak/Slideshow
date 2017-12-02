using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace Slideshow
{
    class ImageController
    {
        private static ImageController instance;
        private Random rnd;
        private Thread thread;
        private int Incrementer;
        public Boolean Run { get; set; }
        public int Timer { get; set; }
        public string[] FileEntries { get; set; }
        public List<ImageView> ImageViews { get; set; }

        public static ImageController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ImageController();
                }
                return instance;
            }
        }

        public ImageController()
        {
            Incrementer = -1;
            Run = false;
            ImageViews = new List<ImageView>();
            rnd = new Random();
        }

        public void Reset()
        {
            Incrementer = -1;
        }

        public void Start()
        {
            Run = true;
            if (thread == null)
            {
                thread = new Thread(Worker)
                {
                    IsBackground = true
                };
                thread.Start();
            }
        }

        public void Stop()
        {
            Run = false;
        }

        public void Worker()
        {
            NextImage();
            Thread.Sleep(Timer);
            if (Run)
            {
                Worker();
            }
            else
            {
                thread = null;
            }
        }

        public void NextImage()
        {
            // Incrementer focus
            Incrementer++;
            if (Incrementer >= FileEntries.Length)
            {
                Incrementer -= FileEntries.Length;
            }
            ChangeImages();
        }

        public void PrevImage()
        {
            // Incrementer focus
            Incrementer--;
            if (Incrementer < 0)
            {
                Incrementer += FileEntries.Length;
            }
            ChangeImages();
        }

        public void ChangeImages()
        {
            for (int i = 0; i < ImageViews.Count; i++)
            {
                ImageView imageView = ImageViews[i];

                //rnd.Next(FileEntries.Length)
                int j = i;
                if (Incrementer + j >= FileEntries.Length)
                {
                    j -= FileEntries.Length;
                }

                // Get filename
                FileInfo fileName = new FileInfo(FileEntries[Incrementer + j]);
                UniversalContenter.ChangeImage(imageView.Content, fileName.FullName);
            }
        }
    }
}
