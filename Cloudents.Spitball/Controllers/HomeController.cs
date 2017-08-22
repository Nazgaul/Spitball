using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Cognitive.LUIS;

namespace Cloudents.Spitball.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Do(string val)
        {
            using (var client = new LuisClient("a5a39777-c674-4ffc-9317-3010b71b76c0",
                "6effb3962e284a9ba73dfb57fa1cfe40"))
            {
                var result = await client.Predict(val).ConfigureAwait(false);
                return new ContentResult()
                {
                    Content = "hi"
                };
            }
        }
    }
}