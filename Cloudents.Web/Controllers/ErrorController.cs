using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using Cloudents.Core.Extension;
using Microsoft.ApplicationInsights;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : Controller
    {
        private readonly TelemetryClient _telemetryClient;

        public ErrorController(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        [ActionName("NotFound")]
        public ActionResult Error404()
        {
            _telemetryClient.TrackTrace("headers - not found",Request.Headers.ToDictionary(x=>x.Key,y=>y.Value.ToString()));
            Response.StatusCode = 404;
            return View("NotFound");
        }

        [Route("error/{code:int}")]
        [Route("error")]
        public ActionResult Index()
        {

            var statusCode = (HttpStatusCode)Response.StatusCode;
            var x = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            if (x == null)
            {
                return RedirectToAction("NotFound");
            }

            // For API errors, responds with just the status code (no page).
            if (x.OriginalPath.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
                return StatusCode((int)statusCode);

            // Creates a view model for a user-friendly error page.
            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    return RedirectToAction("NotFound");

                case HttpStatusCode.Unauthorized:
                    return Redirect("/");
            }
            //return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorText = text });

            Response.StatusCode = 500;
            return View();
        }
    }
}
