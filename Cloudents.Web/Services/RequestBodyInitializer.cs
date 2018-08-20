using System.IO;
using Cloudents.Core.Entities.Db;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Identity;

namespace Cloudents.Web.Services
{
    public class RequestBodyInitializer :  ITelemetryInitializer
    {
        readonly IHttpContextAccessor _httpContextAccessor;

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
            _httpContextAccessor.HttpContext.Request.EnableRewind();

            var stream = new StreamReader(_httpContextAccessor.HttpContext.Request.Body);
            var body = stream.ReadToEnd();

            //Reset the stream so data is not lost
            _httpContextAccessor.HttpContext.Request.Body.Position = 0;
            requestTelemetry.Properties.Add(jsonBody, body);
        }
    }

    public class UserIdInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public UserIdInitializer(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry is RequestTelemetry)
            {
                if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    telemetry.Context.User.Id = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
                }
            }
        }
    }
}