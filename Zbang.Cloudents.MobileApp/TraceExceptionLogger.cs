using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using Microsoft.ApplicationInsights;

namespace Zbang.Cloudents.MobileApp
{
    public class TraceExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            Trace.TraceError(context.ExceptionContext.Exception.ToString());
        }

        public virtual Task LogAsyncCore(ExceptionLoggerContext context,
                                     CancellationToken cancellationToken)
        {
            Log(context);
            return Task.FromResult(0);
        }
    }

    public class UnhandledExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var telemetryClient = new TelemetryClient();
            telemetryClient.TrackException(actionExecutedContext.Exception);
            Trace.TraceError(actionExecutedContext.Exception.ToString());
            base.OnException(actionExecutedContext);
        }

        public override Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            var telemetryClient = new TelemetryClient();
            telemetryClient.TrackException(actionExecutedContext.Exception);
            Trace.TraceError(actionExecutedContext.Exception.ToString());
            return base.OnExceptionAsync(actionExecutedContext, cancellationToken);
        }
    }
}