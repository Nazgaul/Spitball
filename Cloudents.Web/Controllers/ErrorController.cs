using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
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

        //[ActionName("NotFound")]
        //[Route("error/404", Order = 1)]
        //public ActionResult Error404([FromQuery]string? track,
        //    [FromHeader(Name = "referer")] string referer)
        //{
        //    var allHeaders = Request.Headers.ToDictionary(x => x.Key, y => y.Value.ToString());
        //    allHeaders.Add("s-referer",referer);
        //    allHeaders.Add("track",track);
        //    _telemetryClient.TrackTrace("Reaching 404 page", allHeaders);
        //    Response.StatusCode = 404;
        //    return View("NotFound");
        //}

        [Route("error/{code:int}", Order = 2)]
        [Route("error")]
        [Route("error/NotFound")]
        public ActionResult Index(int? code,
            [FromHeader(Name = "referer")] string referer)
        {
            var allHeaders = Request.Headers.ToDictionary(x2 => x2.Key, y => y.Value.ToString());
            allHeaders.Add("s-referer",referer);

            var statusCode = (HttpStatusCode)Response.StatusCode;
            var x = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            if (x == null)
            {
                _telemetryClient.TrackTrace($"Reaching error page {code}", allHeaders);
                Response.StatusCode = 404;
                return View("NotFound");
                //return RedirectToAction("NotFound");
            }

            allHeaders.Add("s-path", x.OriginalPath);
            allHeaders.Add("s-query", x.OriginalQueryString);
            _telemetryClient.TrackTrace($"Reaching error page {code}", allHeaders);
         
            // For API errors, responds with just the status code (no page).
            if (x.OriginalPath.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
                return StatusCode((int)statusCode);

         
            // Creates a view model for a user-friendly error page.
            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    Response.StatusCode = 404;
                    return View("NotFound");
                    //return RedirectToAction("NotFound",new
                    //{
                    //    track = System.Diagnostics.Activity.Current.RootId
                    //});

                case HttpStatusCode.Unauthorized:
                    return Redirect("/");
            }
            //return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorText = text });

            Response.StatusCode = 500;
            return View();
        }
    }
}
