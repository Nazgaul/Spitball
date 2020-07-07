using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RedirectController : Controller
    {
        [Route("ask")]
        [Route("tutor")]
        [Route("flashcard")]
        [Route("note")]
        public IActionResult Index()
        {
            return RedirectPermanent("/");
        }

        [Route("api/locale")]
        public IActionResult Locale()
        {
            return Ok();
        }
    }
}
