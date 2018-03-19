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
        [HttpGet("{brigade}/{code}")]
        public IActionResult Get(int brigade, int code)
        {
            return File(Video.GetVideo(brigade, code), "image/jpeg");
        }
    }
}
