using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Zbang.Zbox.Infrastructure;

namespace Cloudents.Core.Spitball.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAi m_Ai;

        public HomeController(IAi ai)
        {
            m_Ai = ai;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Predict(string str)
        {
            await m_Ai.InterpetString(str);
            return Content("hi");
        }
    }
}