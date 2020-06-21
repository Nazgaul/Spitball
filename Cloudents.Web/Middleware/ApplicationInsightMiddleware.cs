using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;

namespace Cloudents.Web.Middleware
{

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Middleware")]
    public class ApplicationInsightMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Lazy<TelemetryClient> _context;

        public ApplicationInsightMiddleware(RequestDelegate next, Lazy<TelemetryClient> context)
        {
            _next = next;
            _context = context;
        }

        // IMyScopedService is injected into Invoke
        public async Task InvokeAsync(HttpContext httpContext)
        {

            // var request = httpContext.Request;
            httpContext.Request.EnableBuffering();
            await _next(httpContext);
            try
            {
                var response = httpContext.Response;
                if (response.StatusCode < 400)
                {
                    return;
                }

                var request = httpContext.Request;
                if (request.Method == HttpMethods.Post || request.Method == HttpMethods.Put)
                {
                    try
                    {
                        //  _context.Context.Operation.Id = System.Diagnostics.Activity.Current.RootId;// httpContext.TraceIdentifier;
                        request.Body.Seek(0, SeekOrigin.Begin);
                        using var stream = new StreamReader(request.Body);
                        var v = await stream.ReadToEndAsync();
                        _context.Value.TrackTrace($"Log of parameters: {v}",SeverityLevel.Error);
                    }
                    catch (ObjectDisposedException)
                    {
                        _context.Value.TrackTrace("Cant log parameters",SeverityLevel.Error);
                    }
                }
            }
            catch (Exception)
            {
                // Console.WriteLine(e);
                //throw;
            }
        }
    }
}