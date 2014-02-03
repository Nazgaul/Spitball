using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace Zbang.Zbox.WebApi.Helpers
{
    public class BuildRouteTable
    {
        public static void BuildRouteTableFromAssemblies(HttpConfiguration config)
        {
            var controllers = Assembly.GetExecutingAssembly().GetExportedTypes().Where(type => typeof(ApiController).IsAssignableFrom(type));
            foreach (var controller in controllers)
            {
                var prefix = string.Empty;
                var prefixAttribute = (RoutingPrefixAttribute[])controller.GetCustomAttributes(typeof(RoutingPrefixAttribute), true);
                if (prefixAttribute != null && prefixAttribute.Length > 0)
                {
                    prefix = prefixAttribute[0].UriPrefix;
                }
                GenerateRouteToMethod(prefix, config, controller);
            }
        }

        private static void GenerateRouteToMethod(string prefix, HttpConfiguration config, Type controller)
        {
            var controllerName = controller.Name.Substring(0, controller.Name.IndexOf("Controller"));
            foreach (var method in controller.GetMethods())
            {
                var action = method.Name;
                var routeing = (RoutingAttribute[])method.GetCustomAttributes(typeof(RoutingAttribute), true);
                var actionNameAttribute = (ActionNameAttribute[])method.GetCustomAttributes(typeof(ActionNameAttribute), true);
                if (actionNameAttribute != null && actionNameAttribute.Length > 0)
                {
                    action = actionNameAttribute[0].Name;
                }

                if (routeing != null && routeing.Length > 0)
                {
                    if (config.Routes.ContainsKey(action))
                    {
                        continue;
                    }
                    config.Routes.MapHttpRoute(action, routeing[0].UriTemplate, new { controller = controllerName, action = action });
                }
            }
        }


    }
}