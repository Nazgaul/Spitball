using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.Http.Routing;
using Cloudents.Core.Extension;
using Cloudents.Mobile.Models;

namespace Cloudents.Mobile.Extensions
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
        public static string Link(this UrlHelper urlHelper, string routeName, object routeValue, object queryString)
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
        public static string NextPageLink(this UrlHelper urlHelper, string routeName, object routeValue, IPaging queryString)
        {
            queryString.Page = queryString.Page.GetValueOrDefault() + 1;
            return Link(urlHelper, routeName, routeValue, queryString);
        }

        //public static class NameValueCollectionExtension
        //{
        private static void AddObject(string prefix, object val, NameValueCollection nvc)
        {
            if (val == null)
            {
                return;// nvc;
            }

            prefix = prefix.TrimStart('.');
            var valType = val.GetType();
            if (val is string)
            {
                nvc.Add(prefix, val.ToString());
                return;
            }
            //if (val.ToString() != val.GetType().ToString())
            //{
            //    //nvc.Add(prefix, val.ToString());
            //    return;
            //}
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
    }
}