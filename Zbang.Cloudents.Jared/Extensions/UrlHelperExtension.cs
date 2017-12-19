﻿using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Http.Routing;
using Cloudents.Core.Extension;
using Zbang.Cloudents.Jared.Filters;
using Zbang.Cloudents.Jared.Models;

namespace Zbang.Cloudents.Jared.Extensions
{
    public static class UrlHelperExtension
    {
        public static string Link(this UrlHelper urlHelper, string routeName, object routeValue, object queryString)
        {
            var url = urlHelper.Link(routeName, routeValue);
            var builder = new UriBuilder(new Uri(url));
            builder.AddQuery(AddObject(queryString));
            return builder.ToString();
        }


        public static string NextPageLink(this UrlHelper urlHelper, string routeName, object routeValue, IPaging queryString)
        {
            queryString.Page = queryString.Page.GetValueOrDefault() + 1;
            return Link(urlHelper, routeName, routeValue, queryString);
        }

        //public static class NameValueCollectionExtension
        //{
        private static NameValueCollection AddObject(object val)
        {
            if (val == null)
            {
                return new NameValueCollection();
            }
            var builder = new NameValueCollection();
            var valType = val.GetType();
            foreach (var property in valType.GetProperties())
            {
                var propertyValue = property.GetValue(val);
                if (propertyValue == null)
                {
                    continue;
                }
                if (propertyValue is string str)
                {
                    builder.Add(property.Name, str);
                    continue;
                }
                if (propertyValue is IEnumerable p)
                {
                    foreach (var z in p)
                    {
                        if (z != null)
                        {
                            builder.Add(property.Name, z.ToString());
                        }
                    }
                }
                builder.Add(property.Name, propertyValue.ToString());
            }
            return builder;
        }
        //}
    }
}