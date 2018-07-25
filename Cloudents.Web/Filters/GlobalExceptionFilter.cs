using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cloudents.Web.Filters
{
    public class GlobalExceptionFilter : IAsyncExceptionFilter
    {
        //private readonly IHostingEnvironment _hostingEnvironment;

        //public GlobalExceptionFilter(IHostingEnvironment hostingEnvironment)
        //{
        //    _hostingEnvironment = hostingEnvironment;
        //}

        

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            string body = null;
            if (string.Equals(context.HttpContext.Request.Method, "post", StringComparison.OrdinalIgnoreCase))
            {
                if (context.HttpContext.Request.Body.CanSeek)
                {
                    context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                    using (var sr = new StreamReader(context.HttpContext.Request.Body))
                    {
                        body = await sr.ReadToEndAsync();

                    }
                }
            }
            var telemetry = new TelemetryClient();
            telemetry.TrackException(context.Exception,new Dictionary<string, string>()
            {
                ["body"] = body
            });
        }
    }
}
