using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Zbang.Cloudents.Jared
{
    public class RequestBodyInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            //if (telemetry is RequestTelemetry requestTelemetry &&
            //    //requestTelemetry.Context.
            //    requestTelemetry.HttpMethod != null &&
            //    (requestTelemetry.HttpMethod == HttpMethod.Post.ToString()
            //    || requestTelemetry.HttpMethod == HttpMethod.Put.ToString()))
            //{
            //    using (var reader = new StreamReader(HttpContext.Current.Request.InputStream))
            //    {
            //        string requestBody = reader.ReadToEnd();
            //        requestTelemetry.Properties.Add("body", requestBody);
            //    }
            //}
        }
    }
}