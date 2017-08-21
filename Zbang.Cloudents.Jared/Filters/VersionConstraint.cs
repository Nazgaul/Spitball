using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Zbang.Cloudents.Jared.Filters
{
    public class xxx : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var t = actionContext.Request.Headers.GetValues("z");
            return base.IsAuthorized(actionContext);
        }
    }

    public class VersionConstraint : IHttpRouteConstraint
    {
        public const string VersionHeaderName = "s-version";
        private const int DefaultVersion = 1;
        public VersionConstraint(int allowedVersion)
        {
            AllowedVersion = allowedVersion;
        }
        public int AllowedVersion
        {
            get;
            private set;
        }
        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            if (routeDirection != HttpRouteDirection.UriResolution) return false;
            var version = GetVersionHeader(request) ?? DefaultVersion;
            if (version == AllowedVersion)
            {
                return true;
            }
            return false;
        }
        private int? GetVersionHeader(HttpRequestMessage request)
        {
            string versionAsString;
            IEnumerable<string> headerValues;
            if (request.Headers.TryGetValues(VersionHeaderName, out headerValues) && headerValues.Count() == 1)
            {
                versionAsString = headerValues.First();
            }
            else
            {
                return null;
            }
            int version;
            if (versionAsString != null && int.TryParse(versionAsString, out version))
            {
                return version;
            }
            return null;
        }
    }
}