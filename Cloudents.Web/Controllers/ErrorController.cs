using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/error/notfound/", Name = SeoTypeString.NotFound)]

        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        [Route("/error/servererror/", Name = SeoTypeString.ServerError)]

        public ActionResult ServerError()
        {
            Response.StatusCode = 500;
            return View();
        }
    }


}
