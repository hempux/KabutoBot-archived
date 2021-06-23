using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Net;
using net.hempux.Controllers;

namespace net.hempux.kabuto
{

    [Route("api/ninja")]
    [ApiController]
    public class NinjaApiController : ControllerBase
    {
       
        
        public async Task<IActionResult> Get()
        {


           await Task.Run(() => Console.WriteLine("ninja.log"));

            // Let the caller know proactive messages have been sent
            return new ContentResult()
            {
                Content = "<html><body><h1>NinJa.</h1></body></html>",
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
            };
        }
    }
}
