using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Cloudents.Web.Models
{
    public class SiteMapNode
    {
        public string Url { get; set; }
        public DateTime? LastModified { get; set; }
        public SiteMapFrequency? Frequency { get; set; }
        public double? Priority { get; set; }

        public IList<SiteMapLangNode> SiteMapLangNodes { get; set; }

        public SiteMapNode(string relativeUrl, HttpRequest request)
        {
            // Url = relativeUrl;
            Url = GetAbsoluteUrl(request, relativeUrl.Trim());
            Priority = null;
            Frequency = null;
            LastModified = null;
        }

        //public SiteMapNode(RequestContext request, string routeName, object routeValues)
        //{
        //    var x = RouteTable.Routes.GetVirtualPathForArea(request, routeName, new RouteValueDictionary(routeValues));
        //    if (x != null)
        //    {
        //        Url = GetAbsoluteUrl(request, x.VirtualPath);
        //    }
        //    Priority = null;
        //    Frequency = null;
        //    LastModified = null;
        //}

        //public static IEnumerable<SitemapNode> SiteMapNodesWithLang(RequestContext request, params SitemapNodeLangHelper[] nodes)
        //{
        //    var siteMapNodes = new List<SitemapNode>();
        //    foreach (var mainNode in nodes)
        //    {
        //        var siteMapNode = new SitemapNode(request, mainNode.RouteName, mainNode.RouteValues)
        //        {
        //            SitemapLangNodes = new List<SitemapLangNode>()
        //        };

        //        foreach (var node in nodes)
        //        {
        //            var url = GetUrl(request, node.RouteName, node.RouteValues);
        //            siteMapNode.SitemapLangNodes.Add(new SitemapLangNode(url, node.Language));
        //        }
        //        siteMapNodes.Add(siteMapNode);
        //    }
        //    return siteMapNodes;
        //}


        //public static string GetUrl(RequestContext request, string routeName, object routeValues)
        //{
        //    var routes = RouteTable.Routes;

        //    var data = routes.GetVirtualPathForArea(request, routeName, new RouteValueDictionary(routeValues));

        //    if (data == null)
        //    {
        //        return null;
        //    }
        //    return GetAbsoluteUrl(request, data.VirtualPath);
        //}

        private static string GetAbsoluteUrl(HttpRequest request, string relativeUrl)
        {
            var baseUrl = request.GetUri();
            return request.HttpContext?.Request != null && baseUrl != null
                       ? new Uri(baseUrl, relativeUrl).AbsoluteUri
                       : null;
        }
    }
}
