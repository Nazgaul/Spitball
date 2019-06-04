using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Web.Filters;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Controllers
{
    public class ProfileController : Controller
    {
        // GET: /<controller>/
        [Route("profile/{id:long}/{name:string}")]
        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = TimeConst.Hour, NoStore = true), SignInWithToken]
        public IActionResult Index(long id)
        {

            return View();
        }
    }
}
