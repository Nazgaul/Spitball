using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TabsController : Controller
    {
        // GET: /<controller>/
        [Route("book")]
        [Route("job")]
        [Route("flashcard")]
        [Route("tutor")]
        [Route("ask")]
        public IActionResult Index()
        {
            return Redirect("/");
        }
    }
}
