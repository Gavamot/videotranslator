using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ConsoleApp1;
using NLog;

namespace ASCManagerWeb2.App_Start
{
    public class CameraCash
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // Singlton
        private CameraCash()
        {
            
        }

        private static readonly CameraCash instance = new CameraCash();
        public static CameraCash Instance => instance;
        // Singlton ---------------------------------------------------
       
        volatile CancellationTokenSource ts;
        volatile object objLock = new object();
        Task[] tasks = null;
        private List<CameraWithImg> camers;

        private void DownloadImg(CameraWithImg item, Uri src)
        {
            try
            {
                using (var client = new WebClient())
                {
                    byte[] data = client.DownloadData(src);
                    item.Img = data;
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Can not downloaded img from {src.AbsolutePath}");
            }
        }

        public byte[] GetImg(int id)
        {
            var c = camers; // AntiRace
            CameraWithImg img = c.FirstOrDefault(x => x.Camera.Id == id);
            return img?.Img;
        }

        public byte[] GetImg(int brigadeId, int cameraNumber)
        {
            var c = camers; // AntiRace
            CameraWithImg img = c.FirstOrDefault(x => x.Camera.BrigadeId == brigadeId && x.Camera.Num == cameraNumber);
            return img?.Img;
        }

        public void BeginUpdate()
        {
            EndUpdate();
            logger.Info("Started");
            lock (objLock)
            {
                camers = GetAllCamers();
                ts = new CancellationTokenSource();

                tasks = new Task[camers.Count];
                for (int i = 0; i < tasks.Length; i++)
                {
                    var item = camers[i];
                    tasks[i]  = Task.Factory.StartNew(async () =>
                    {
                        // Пытаемся распарсить Uri
                        Uri src;
                        try
                        {
                            src = new Uri(item.Camera.Src);
                        }
                        catch (Exception e) // Если не получилось то прерываем операцию
                        { 
                            logger.Error(e, $"Uri({ item.Camera.Src }) is not correct");
                            return;
                        }
                        //  
                        while (!ts.Token.IsCancellationRequested)
                        {
                            DownloadImg(item, src);
                            //await DownloadImgAsync(item, src);
                            if (item.Camera.SleepMs > 0)
                            {
                                await Task.Delay(item.Camera.SleepMs);
                            }
                        }
                    }, ts.Token);
                }
            }
        }

        // Предратить обновление с камер
        public void EndUpdate()
        {
            lock (objLock)
            {
                ts?.Cancel();
                // Если задачи есть ждем пока они завершатся
                if ( tasks != null )
                {
                    try
                    {
                        Task.WaitAll(tasks);
                    }
                    catch (Exception e)
                    {
                        
                    }
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