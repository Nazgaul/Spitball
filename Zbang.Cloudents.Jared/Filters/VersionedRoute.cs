using System.Collections.Generic;
using System.Web.Http.Routing;

namespace Zbang.Cloudents.Jared.Filters
{
    public class VersionedRouteAttribute : RouteFactoryAttribute
    {
        public VersionedRouteAttribute(string template, string allowedVersion)
            : base(template)
        {
            AllowedVersion = allowedVersion;
        }
        public string AllowedVersion
        {
            get;
        }
        public override IDictionary<string, object> Constraints
        {
            get
            {
                var constraints = new HttpRouteValueDictionary {["version"] = new VersionConstraint(AllowedVersion) };
                return constraints;
            }
        }
    }
}