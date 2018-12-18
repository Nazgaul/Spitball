using System;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class QuizController : Controller
    {
        [Route("quiz/{universityName}/{boxId:long}/{boxName}/{id:long}/{name}")]
        public IActionResult Index()
        {
            var path = HttpContext.Request.Path.Value.TrimEnd('/');
            var uriBuilder = new UriBuilder
            {
                Scheme = "https",
                Host = "heb.spitball.co",
                Path = path + "/",
                Query = HttpContext.Request.QueryString.Value
            };
            return Redirect(uriBuilder.ToString());
            // return this.RedirectToOldSite();
        }
    }
}