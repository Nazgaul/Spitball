using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Zbang.Cloudents.Jared
{
    internal class RequestBodyInitializer : ITelemetryInitializer
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