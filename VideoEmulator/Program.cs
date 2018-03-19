using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace VideoEmulator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //const string root = @"C:\Users\user\Documents\Visual Studio 2017\Projects\ConsoleApp1\VideoEmulator\Images\";
            //RenameFiles(root);
            BuildWebHost(args).Run();
        }

        public static void RenameFiles(string root)
        {
            var dir = new DirectoryInfo(root);
            int i = 1;
            foreach (var f in dir.GetFiles())
            {
                string name = "img_" + i + f.Extension;
                File.Move(f.FullName, root + name);
                i++;
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls($"http://*:1336")
                .Build();
    }
}
