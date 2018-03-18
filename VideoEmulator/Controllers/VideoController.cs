using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace VideoEmulator.Controllers
{
    [Route("[controller]")]
    public class VideoController : Controller
    {
        // GET controller/5
        [HttpGet("{src}")]
        public IActionResult Get(string src)
        {
            return File(Video.GetVideo(src), "image/jpeg");
        }
    }
}
