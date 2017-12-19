using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Zbang.Cloudents.Jared.Filters
{
    public class VersionConstraint : IHttpRouteConstraint
    {
        public const string VersionHeaderName = "api-version";
        //private const int DefaultVersion = 1;
        public VersionConstraint(string allowedVersion)
        {
            AllowedVersion = allowedVersion;
        }
        public string AllowedVersion
        {
            get;
        }
        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            if (routeDirection != HttpRouteDirection.UriResolution) return true;
            var version = GetVersionHeader(request);// ?? DefaultVersion;
            if (version == AllowedVersion)
            {
                return true;
            }
            return false;
        }
        private static string GetVersionHeader(HttpRequestMessage request)
        {
            //string versionAsString;
            var queryString = request.RequestUri.ParseQueryString();
            return queryString[VersionHeaderName];


            //if (request.Headers.TryGetValues(VersionHeaderName, out var headerValues) && headerValues.Count() == 1)
            //{
            //    versionAsString = headerValues.First();
            //}
            //else
            //{
            //    return null;
            //}
            //if (versionAsString != null && int.TryParse(versionAsString, out int version))
            //{
            //    return version;
            //}
            //return null;
        }
    }
}