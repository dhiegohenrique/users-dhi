using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UsersDhi.Controllers
{
    [Route("/")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class IndexController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Redirect("swagger");
        }
    }
}