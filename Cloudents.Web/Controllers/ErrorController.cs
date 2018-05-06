using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : Controller
    {
        [ActionName("NotFound")]
        public ActionResult Error404()
        {
            //Response.StatusCode = 404;
            return View("NotFound");
        }

        public ActionResult Index()
        {
            //Response.StatusCode = 500;
            return View();
        }
    }
}
