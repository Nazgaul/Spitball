using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class QuestionController : Controller
    {
        // GET
        [Route("[controller]/{id}", Name = "QuestionRoute")]
        public IActionResult Index()
        {
            return
            View();
        }
    }
}