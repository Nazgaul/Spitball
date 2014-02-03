using System.Web.Http;
using Zbang.Zbox.WebApi.Helpers;

namespace Zbang.Zbox.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            RouteV1(config);
        }

        private static void RouteV1(HttpConfiguration config)
        {
            BuildRouteTable.BuildRouteTableFromAssemblies(config);
            //config.Routes.MapHttpRoute(name: "GetBoxes", routeTemplate: "Boxes", defaults: new { controller = "Zbox", action = "GetUserBoxes" });
            //config.Routes.MapHttpRoute(name: "LogInMembership", routeTemplate: "LogIn", defaults: new { controller = "Zbox", action = "LogInUserMembership" });
            //config.Routes.MapHttpRoute(name: "GetBox", routeTemplate: "Boxes/{boxid}", defaults: new { controller = "Zbox", action = "GetBox", boxid = RouteParameter.Optional });
            //config.Routes.MapHttpRoute("GetBoxUsers", "Boxes/{boxid}/Users", new { controller = "Zbox", action = "GetBoxUsers", boxid = RouteParameter.Optional });
        }

    }

    

}
