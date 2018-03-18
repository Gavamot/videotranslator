using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VideoEmulator
{
    public static class Video
    {
        public static readonly List<byte[]> Images;
        public static Random rnd = new Random(DateTime.Now.Millisecond);
        public static readonly int ImgCount;
        static Video() // Read all files to memory.
        {
            var files = new List<byte[]>();
            foreach (var fd in Directory.GetFiles("./Images"))
            {
                byte[] f = File.ReadAllBytes(fd);
                files.Add(f);
            }
            Images = files;
            ImgCount = files.Count;
        }

        public static byte[] GetVideo(string src)
        {
            return Images[rnd.Next(0, ImgCount)];
        }
    }
}
