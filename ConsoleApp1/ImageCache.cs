using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using NLog;

namespace ASCManagerWeb2.App_Start
{
    public class ImageCache
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private ImageCache()
        {
            
        }

        private static readonly ImageCache instance = new ImageCache();
        public static ImageCache Instance => instance;
        private readonly Cacher cacher = new Cacher();
        private Thread thread;

        public void Start()
        {
            thread = new Thread(cacher.BeginUpdate);
            thread.Start();
        }

        private void Stop()
        {
            
        }

        private class Cacher
        {
            volatile CancellationTokenSource ts;
            volatile object objLock = new object();
            Task[] tasks = null;

            private async void DownloadImgAsync(CameraWithImg item, Uri src)
            {
                using (var client = new WebClient())
                {
                    item.Img = await client.DownloadDataTaskAsync(src);
                }
            } 

            public void BeginUpdate()
            {
                EndUpdate();
                logger.Info("Started");
                lock (objLock)
                {
                    List<CameraWithImg> camers = GetAllCamers();
                    ts = new CancellationTokenSource();

                    tasks = new Task[camers.Count];
                    for (int i = 0; i < tasks.Length; i++)
                    {
                        var item = camers[i];
                        tasks[i]  = Task.Factory.StartNew(() =>
                        {
                            // Пытаемся распарсить Uri
                            Uri src;
                            try
                            {
                                src = new Uri(item.Camera.Src);
                            }
                            catch (Exception e) // Если не получилось то прерываем операцию
                            { 
                                logger.Error(e, $"Uri({item.Camera.Src}) is not correct");
                                return;
                            }
                            
                            // 
                            while (ts.Token.IsCancellationRequested)
                            {
                                DownloadImgAsync(item, src);
                                Task.Delay(item.Camera.SleepMs);
                            }
                        }, ts.Token);
                    }
                }
            }

            // Предратить обновление с камер
            private void EndUpdate()
            {
                lock (objLock)
                {
                    ts?.Cancel();
                    // Если задачи есть ждем пока они завершатся
                    if (tasks != null)
                    {
                        Task.WaitAll(tasks);
                    }
                }
                logger.Info("Stoped");
            }

            List<CameraWithImg> GetAllCamers()
            {
                try
                {
                    var res = Camera.GetAll()
                        .Select(x => new CameraWithImg(x))
                        .ToList();
                    return res;
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                }
                return new List<CameraWithImg>();
            }

            class CameraWithImg
            {
                public CameraWithImg() { }
                public CameraWithImg(Camera camera)
                {
                    Camera = camera;
                }

                public Camera Camera { get; set; }
                public byte[] Img { get; set; }
            }
        }
    }

    public class Camera
    {
        public static List<Camera> GetAll()
        {
            return new List<Camera>
            {
                new Camera{Src = "", SleepMs = 5000},
                new Camera{Src = "", SleepMs = 5000}
            };
        }

        public string Src { get; set; }
        public int SleepMs { get; set; }
    }
}