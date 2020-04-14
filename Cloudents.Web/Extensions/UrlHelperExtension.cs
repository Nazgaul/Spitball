using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
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
            queryString.Page += 1;
            return Link(urlHelper, routeName, routeValue, queryString);
        }

        //public static string NextPageLink(this IUrlHelper urlHelper, string routeName, IPaging queryString)
        //{
        //    return NextPageLink(urlHelper, routeName, null, queryString);
        //}

        private static void AddObject(string prefix, object val, NameValueCollection nvc)
        {
            if (val == null)
            {
                return;
            }

            prefix = prefix.TrimStart('.');
            var valType = val.GetType();

            //if (valType.GetAttribute<IgnoreNextPageLinkAttribute>() != null)
            //{
            //    return;

            //}

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
                var z = property.GetCustomAttribute(typeof(IgnoreNextPageLinkAttribute), true);
                if (z != null)
                {
                    continue;
                }
                var propertyValue2 = property.GetValue(val);

                AddObject($"{prefix}.{property.Name}", propertyValue2, nvc);
            }
        }

        private static bool IsAnonymous(Type valType)
        {
            return valType.Namespace != null;
        }


        public static string DocumentUrl(this IUrlHelper helper,   long id, string name)
        {
            return helper.RouteUrl(SeoTypeString.Document, new
            {
                id,
                name = FriendlyUrlHelper.GetFriendlyTitle(name)
            });
        }


        public static string ImageUrl(this IUrlHelper helper,
            ImageProperties properties)
        {
            var serializer = helper.ActionContext.HttpContext.RequestServices.GetRequiredService<IBinarySerializer>();
            var hash = serializer.Serialize(properties);
            return helper.RouteUrl("imageUrl", new
            {
                hash = Base64UrlTextEncoder.Encode(hash)
            });
        }
    }

    //public static class UriExtensions
    //{
    //    public static string RemoveQueryStringByKey(this Uri uri, string key)
    //    {

    //        var newQueryString = QueryHelpers.ParseQuery(uri.Query);
    //        // this gets all the query string key value pairs as a collection
    //        //var newQueryString = HttpUtility.ParseQueryString(uri.Query);

    //        // this removes the key if exists
    //        newQueryString.Remove(key);

    //        // this gets the page path from root without QueryString
    //        string pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

    //        return newQueryString.Count > 0
    //            ? String.Format("{0}?{1}", pagePathWithoutQueryString, newQueryString)
    //            : pagePathWithoutQueryString;
    //    }
    //}

    public sealed class IgnoreNextPageLinkAttribute : Attribute
    {

    }
}