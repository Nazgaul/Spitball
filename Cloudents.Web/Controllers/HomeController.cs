using System;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }

        //[HttpPost("Ai")]
        //public async Task<IActionResult> AiAsync([FromBody] AI sentence, [FromServices] IAI luis)
        //{
        //    if (sentence == null) throw new ArgumentNullException(nameof(sentence));
        //    var result = await luis.InterpetStringAsync(sentence.Sentence);
        //    return Content("hi");
        //}
    }
}