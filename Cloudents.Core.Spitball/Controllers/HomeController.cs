using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Core.Spitball.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet, ActionName("search")]
        public IActionResult Search()
        {
            int a = 5;
            a = 7;
            return Json("Yifat");
        }
    }
}