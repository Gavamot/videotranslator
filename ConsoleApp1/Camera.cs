using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Camera
    {
        public static IEnumerable<Camera> GetAll()
        {
            for (int i = 0; i < 5; i++)
            {
               yield return new Camera {BrigadeId = i, Num = 1, Src = $@"http://localhost:1336/video/{i}/1", SleepMs = i};
            }
        }

        public int Id { get; set; }
        public int Num { get; set; }
        public string Name { get; set; }
        public int BrigadeId { get; set; }
        public string Src { get; set; }
        public int SleepMs { get; set; }
    }
}
