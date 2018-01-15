using System.Collections.Generic;
using System.Web.Http.Routing;

namespace Cloudents.Mobile.Filters
{
    internal class VersionedRouteAttribute : RouteFactoryAttribute
    {
        public VersionedRouteAttribute(string template)
            : base(template)
        {
           
        }
        public VersionedRouteAttribute(string template, string allowedVersion)
            : base(template)
        {
            AllowedVersion = allowedVersion;
        }
        public string AllowedVersion
        {
            get;
        }
        public override IDictionary<string, object> Constraints => new HttpRouteValueDictionary {["version"] = new VersionConstraint(AllowedVersion) };
    }
}