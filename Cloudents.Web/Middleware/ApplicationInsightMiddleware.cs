using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

//using Microsoft.AspNetCore.Http.Internal;

namespace Cloudents.Web.Middleware
{

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Middleware")]
    public class ApplicationInsightMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApplicationInsightMiddleware> _context;

        public ApplicationInsightMiddleware(RequestDelegate next, ILogger<ApplicationInsightMiddleware> context)
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
                        _context.LogError($"Log of parameters: {v}");
                    }
                    catch (ObjectDisposedException)
                    {
                        _context.LogError("Cant log parameters");
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

    //public class RequestBodyInitializer : ITelemetryInitializer
    //{
    //    private readonly IHttpContextAccessor _httpContextAccessor;

    //    public RequestBodyInitializer(IHttpContextAccessor httpContextAccessor)
    //    {
    //        _httpContextAccessor = httpContextAccessor;
    //    }

    //    public void Initialize(ITelemetry telemetry)
    //    {
    //        if (!(telemetry is RequestTelemetry requestTelemetry) ||
    //            (_httpContextAccessor.HttpContext.Request.Method != HttpMethods.Post &&
    //             _httpContextAccessor.HttpContext.Request.Method != HttpMethods.Put) ||
    //            !_httpContextAccessor.HttpContext.Request.Body.CanRead) return;
    //        const string jsonBody = "JsonBody";

    //        if (requestTelemetry.Properties.ContainsKey(jsonBody))
    //        {
    //            return;
    //        }

    //        //Allows re-usage of the stream
    //        try
    //        {
    //            _httpContextAccessor.HttpContext.Request.EnableBuffering();

    //            if (_httpContextAccessor.HttpContext.Request.Body.CanRead)
    //            {
    //                using (var stream = new StreamReader(_httpContextAccessor.HttpContext.Request.Body))
    //                {
    //                    var body = stream.ReadToEndAsync().Result;
    //                    _httpContextAccessor.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
    //                    requestTelemetry.Properties.Add(jsonBody, body);
    //                }
    //            }
    //        }
    //        catch (ObjectDisposedException)
    //        {
    //            //Do nothing
    //        }
    //    }
    //}

    //public class InputStreamAlwaysBufferedPolicySelector : WebHostBufferPolicySelector
    //{
    //    public override bool UseBufferedInputStream(object hostContext)
    //    {
    //        return true;
    //    }
    //}
}