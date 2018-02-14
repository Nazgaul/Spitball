using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cloudents.Web.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var telemetry = new TelemetryClient();
            telemetry.TrackException(context.Exception);
        }
    }
}
