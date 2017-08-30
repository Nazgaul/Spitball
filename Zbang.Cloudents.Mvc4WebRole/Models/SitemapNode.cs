using System;
using System.Collections.Generic;
using System.Web.Routing;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class SitemapNodeLangHelper
    {
        public SitemapNodeLangHelper(string routeName, object routeValues, string language)
        {
            Language = language;
            RouteName = routeName;
            RouteValues = routeValues;
        }

        public string Language { get;private set; }
        public string RouteName { get;private set; }
        public object RouteValues { get;private set; }
    }

    public class SitemapLangNode
    {
        public SitemapLangNode(string url, string language)
        {
            Url = url;
            Language = language;
        }

        public string Url { get; private set; }
        public string Language { get; private set; }
    }

    public class SitemapNode
    {
        public string Url { get; set; }
        public DateTime? LastModified { get; set; }
        public SitemapFrequency? Frequency { get; set; }
        public double? Priority { get; set; }

        public IList<SitemapLangNode> SitemapLangNodes { get; set; }

        public SitemapNode(string relativeUrl, RequestContext request)
        {
            // Url = relativeUrl;
            Url = GetAbsoluteUrl(request, relativeUrl.Trim());
            Priority = null;
            Frequency = null;
            LastModified = null;
        }

        public SitemapNode(RequestContext request, string routeName, object routeValues)
        {
            var x = RouteTable.Routes.GetVirtualPathForArea(request, routeName, new RouteValueDictionary(routeValues));
            if (x != null)
            {
                Url = GetAbsoluteUrl(request, x.VirtualPath);
            }
            Priority = null;
            Frequency = null;
            LastModified = null;
        }

        public static IEnumerable<SitemapNode> SiteMapNodesWithLang(RequestContext request, params SitemapNodeLangHelper[] nodes)
        {
            var siteMapNodes = new List<SitemapNode>();
            foreach (var mainNode in nodes)
            {
                var siteMapNode = new SitemapNode(request, mainNode.RouteName, mainNode.RouteValues)
                {
                    SitemapLangNodes = new List<SitemapLangNode>()
                };

                foreach (var node in nodes)
                {
                    var url = GetUrl(request, node.RouteName, node.RouteValues);
                    siteMapNode.SitemapLangNodes.Add(new SitemapLangNode(url, node.Language));
                }
                siteMapNodes.Add(siteMapNode);
            }
            return siteMapNodes;
        }


        public static string GetUrl(RequestContext request, string routeName, object routeValues)
        {
            var routes = RouteTable.Routes;

            var data = routes.GetVirtualPathForArea(request, routeName, new RouteValueDictionary(routeValues));

            if (data == null)
            {
                return null;
            }
            return GetAbsoluteUrl(request, data.VirtualPath);
        }

        private static string GetAbsoluteUrl(RequestContext request, string relativeUrl)
        {
            var baseUrl = request.HttpContext.Request.Url;
            return request.HttpContext?.Request != null && baseUrl != null
                       ? new Uri(baseUrl, relativeUrl).AbsoluteUri
                       : null;
        }
    }
}