using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using Microsoft.ApplicationInsights;

namespace Zbang.Cloudents.Jared
{
    public class AiExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            if (context?.Exception != null)
            {//or reuse instance (recommended!). see note above
                var ai = new TelemetryClient();
                ai.TrackException(context.Exception);
            }
            base.Log(context);
        }
        public override async Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            if (context?.Exception != null)
            {//or reuse instance (recommended!). see note above
                var ai = new TelemetryClient();
                var str = await context.Request.Content.ReadAsStringAsync().ConfigureAwait(false);
                ai.TrackException(context.Exception, new Dictionary<string, string> {["content"] = str });
            }
            await base.LogAsync(context, cancellationToken).ConfigureAwait(false);
        }
    }
}