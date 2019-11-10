using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Cloudents.Web.Filters
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Mvc Filter")]
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public GlobalExceptionFilter()
        {
            Order = 999;
        }

        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.ExceptionHandled)
            {
                return;
            }
            string body = null;
            if (string.Equals(context.HttpContext.Request.Method, "post", StringComparison.OrdinalIgnoreCase))
            {
                if (context.HttpContext.Request.Body.CanSeek)
                {
                    try
                    {
                        context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                        using (var sr = new StreamReader(context.HttpContext.Request.Body))
                        {
                            body = await sr.ReadToEndAsync();
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        //do nothing
                    }
                }
            }
            var telemetry = new TelemetryClient();
            telemetry.TrackException(context.Exception, new Dictionary<string, string>()
            {
                ["body"] = body
            });
        }


    }
}
