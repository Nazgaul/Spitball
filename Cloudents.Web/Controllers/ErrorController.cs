using Cloudents.Core.Enum;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    public class ErrorController : Controller
    {

        public ActionResult NotFound()
        {
            //Response.StatusCode = 404;
            return View();
        }

        public ActionResult Index()
        {
            //Response.StatusCode = 500;
            return View();
        }
    }


}
