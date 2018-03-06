using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cloudents.Web.Extensions.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public GlobalExceptionFilter(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public void OnException(ExceptionContext context)
        {
            if (_hostingEnvironment.IsDevelopment())
            {
                return;
            }
            var telemetry = new TelemetryClient();
            telemetry.TrackException(context.Exception);
        }
    }
}
