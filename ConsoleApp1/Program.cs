using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ASCManagerWeb2.App_Start;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var cache = CameraCash.Instance;
            cache.BeginUpdate();
            var camers = Camera.GetAll();
            while (true)
            {
                Task.Run(async () =>
                {
                    while (true)
                    {
                        foreach (var cam in camers)
                        {
                            try
                            {
                                var f = cache.GetImg(cam.BrigadeId, cam.Num);
                                Console.WriteLine($"cam[{cam.BrigadeId}, {cam.Num}]={f.Length}");
                            }
                            catch (Exception e)
                            {

                            }
                        }
                        Console.WriteLine("-----------------------------------------------");
                        await Task.Delay(1000);
                    }
                   
                });

                while (true)
                {
                    var c = Console.ReadKey();
                    if (c.Key == ConsoleKey.Q)
                    {
                        CameraCash.Instance.EndUpdate();
                        Thread.Sleep(1000);
                    }
                    if (c.Key == ConsoleKey.S)
                    {
                        CameraCash.Instance.BeginUpdate();
                        Thread.Sleep(1000);
                    }
                }
                

            }
        }
    }
}
