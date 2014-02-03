using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class SitemapNode
    {
        public string Url { get; set; }
        public DateTime? LastModified { get; set; }
        public SitemapFrequency? Frequency { get; set; }
        public double? Priority { get; set; }


        public SitemapNode(string relativeUrl, RequestContext request)
        {
            // Url = relativeUrl;
            Url = GetAbsoluteUrl(request, relativeUrl.Trim());
            Priority = null;
            Frequency = null;
            LastModified = null;
        }

        public SitemapNode(RequestContext request, object routeValues)
        {
            Url = GetUrl(request, new RouteValueDictionary(routeValues));
            Priority = null;
            Frequency = null;
            LastModified = null;
        }

        private string GetUrl(RequestContext request, RouteValueDictionary values)
        {
            var routes = RouteTable.Routes;
            var data = routes.GetVirtualPathForArea(request, values);

            if (data == null)
            {
                return null;
            }
            return GetAbsoluteUrl(request, data.VirtualPath);
        }

        private string GetAbsoluteUrl(RequestContext request, string relativeUrl)
        {
            var baseUrl = request.HttpContext.Request.Url;
            return request.HttpContext != null &&
                   (request.HttpContext.Request != null && baseUrl != null)
                       ? new Uri(baseUrl, relativeUrl).AbsoluteUri
                       : null;
        }
    }

    public enum SitemapFrequency
    {
        Never,
        Yearly,
        Monthly,
        Weekly,
        Daily,
        Hourly,
        Always
    }
}