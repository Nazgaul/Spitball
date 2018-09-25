using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cloudents.Admin.Models;
using System.IO;

namespace Cloudents.Admin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(),
                            "wwwroot", "dist", "index.html");

            return PhysicalFile(file, "text/html");
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
