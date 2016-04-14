using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

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
}