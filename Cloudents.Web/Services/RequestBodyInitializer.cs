using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System.IO;

namespace Cloudents.Web.Services
{
    public class RequestBodyInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestBodyInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (!(telemetry is RequestTelemetry requestTelemetry) ||
                (_httpContextAccessor.HttpContext.Request.Method != HttpMethods.Post &&
                 _httpContextAccessor.HttpContext.Request.Method != HttpMethods.Put) ||
                !_httpContextAccessor.HttpContext.Request.Body.CanRead) return;
            const string jsonBody = "JsonBody";

            if (requestTelemetry.Properties.ContainsKey(jsonBody))
            {
                return;
            }

            //Allows re-usage of the stream
            try
            {
                _httpContextAccessor.HttpContext.Request.EnableRewind();

                if (_httpContextAccessor.HttpContext.Request.Body.CanRead)
                {
                    using (var stream = new StreamReader(_httpContextAccessor.HttpContext.Request.Body))
                    {
                        var body = stream.ReadToEnd();
                        _httpContextAccessor.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                        requestTelemetry.Properties.Add(jsonBody, body);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
               //Do nothing
            }
        }
    }
}