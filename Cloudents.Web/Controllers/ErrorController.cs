using System;
using System.Diagnostics;
using System.Net;
using System.Web;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
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
            var error = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
            var statusCode = (HttpStatusCode)Response.StatusCode;

            // For API errors, responds with just the status code (no page).
            if (HttpContext.Features.Get<IHttpRequestFeature>().RawTarget.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
                return StatusCode((int)statusCode);

            // Creates a view model for a user-friendly error page.
            string text = null;
            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    return RedirectToAction("NotFound");
            }
            //return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorText = text });

            //Response.StatusCode = 500;
            return View();
        }
    }
}
