﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Specialized;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using IPaging = Cloudents.Web.Models.IPaging;

namespace Cloudents.Web.Extensions
{
    /// <summary>
    /// Extension class for urlHelper
    /// </summary>
    public static class UrlHelperExtension
    {
        /// <summary>
        /// Generate link with query string
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="routeName">the route we want</param>
        /// <param name="routeValue">route object</param>
        /// <param name="queryString">query string</param>
        /// <returns>link</returns>
        private static string Link(this IUrlHelper urlHelper, string routeName, object routeValue, object queryString)
        {
            var url = urlHelper.Link(routeName, routeValue);
            url = url.Replace("_", "-");
            var builder = new UriBuilder(new Uri(url));
            var nvc = new NameValueCollection();
            AddObject(string.Empty, queryString, nvc);
            builder.AddQuery(nvc);
            return builder.ToString();
        }

        /// <summary>
        /// Generate next page link for paging
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="routeName">the route we want</param>
        /// <param name="routeValue">route object</param>
        /// <param name="queryString">query string</param>
        /// <returns>link</returns>
        public static string NextPageLink(this IUrlHelper urlHelper, string routeName, object routeValue, IPaging queryString)
        {
            queryString.Page = queryString.Page.GetValueOrDefault() + 1;
            return Link(urlHelper, routeName, routeValue, queryString);
        }

        public static string NextPageLink(this IUrlHelper urlHelper, string routeName, IPaging queryString)
        {
            return NextPageLink(urlHelper, routeName, null, queryString);
        }

        private static void AddObject(string prefix, object val, NameValueCollection nvc)
        {
            if (val == null)
            {
                return;
            }

            prefix = prefix.TrimStart('.');
            var valType = val.GetType();

            if (val.ToString() != val.GetType().ToString() && IsAnonymous(valType))
            {
                nvc.Add(prefix, val.ToString().Trim());
                return;
            }
            if (val is IEnumerable p)
            {
                foreach (var z in p)
                {
                    if (z != null)
                    {
                        nvc.Add(prefix, z.ToString());
                    }
                }
                return;
            }
            foreach (var property in valType.GetProperties())
            {
                var propertyValue2 = property.GetValue(val);

                AddObject($"{prefix}.{property.Name}", propertyValue2, nvc);
            }
        }

        private static bool IsAnonymous(Type valType)
        {
            return valType.Namespace != null;
        }


        public static string DocumentUrl(this IUrlHelper helper, string university, string course, long id, string name)
        {
            return helper.RouteUrl(SeoTypeString.Document, new
            {
                universityName = university.Replace("+", "-"),
                courseName = course,
                id,
                name = name.Replace("+", "-")
            });
        }
    }
}