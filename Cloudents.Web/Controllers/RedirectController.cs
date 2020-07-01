using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RedirectController : Controller
    {
        [Route("ask")]
        public IActionResult Index()
        {
            return Redirect("/");
        }
    }
}
