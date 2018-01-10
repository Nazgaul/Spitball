using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cloudents.Web.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var telemetry = new TelemetryClient();
            //var properties = new Dictionary<string, string> { { "custom-property1", "property1-value" } };
            telemetry.TrackException(context.Exception);
        }
    }
}
