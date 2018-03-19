using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VideoEmulator
{
    public static class Video
    {
        static Video()
        {
            videos = ReadVideos();
        }

        const string Folder = "./Images";
        static readonly List<byte[]> videos;
        static readonly Random rnd = new Random(DateTime.Now.Millisecond);

        static List<byte[]> ReadVideos()
        {
            var dir = new DirectoryInfo(Folder);
            List<byte[]> res = new List<byte[]>();
            foreach (var fd in dir.GetFiles())
            {
                byte[] f = File.ReadAllBytes(fd.FullName);
                res.Add(f);
            }
            return res;
        }

        public static byte[] GetVideo(int brigade, int code)
        {
            return videos[rnd.Next(0, videos.Count)];
        }
    }
}
