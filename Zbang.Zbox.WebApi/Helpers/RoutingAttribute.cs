using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zbang.Zbox.WebApi.Helpers
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RoutingAttribute :Attribute
    {
        public string UriTemplate { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RoutingPrefixAttribute : Attribute
    {
        public string UriPrefix { get; set; }
    }
}