using System.Collections.Generic;
using System.Web.Http.Routing;

namespace Zbang.Cloudents.Jared.Filters
{
    public class VersionedRoute : RouteFactoryAttribute
    {
        public VersionedRoute(string template, int allowedVersion)
            : base(template)
        {
            AllowedVersion = allowedVersion;
        }
        public int AllowedVersion
        {
            get;
            private set;
        }
        public override IDictionary<string, object> Constraints
        {
            get
            {
                var constraints = new HttpRouteValueDictionary { { "version", new VersionConstraint(AllowedVersion) } };
                return constraints;
            }
        }
    }
}