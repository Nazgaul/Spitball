using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
{
    public static class UrlHelperExtension
    {
        public static string CdnContent(this UrlHelper urlHelper, string contentPath)
        {
            if (BundleConfig.IsDebugEnabled())
            {
                return urlHelper.Content(contentPath);
            }
            var contentTypeAbsolutePath = VirtualPathUtility.ToAbsolute(contentPath);
            if (contentTypeAbsolutePath[0] == '/')
            {
                contentTypeAbsolutePath = contentTypeAbsolutePath.Remove(0, 1);
            }
            var cdnUrl = VirtualPathUtility.AppendTrailingSlash(BundleConfig.CdnEndpointUrl) + contentTypeAbsolutePath;
            return urlHelper.Content(cdnUrl);
        }

        public static string ActionHash(this UrlHelper urlHelper, string action, object routeValues, string hash)
        {
            return string.Format("{0}#{1}", urlHelper.Action(action, routeValues), hash);
        }
        public static string ActionHash(this UrlHelper urlHelper, string action, string controller, string hash)
        {
            return string.Format("{0}#{1}", urlHelper.Action(action, controller), hash);
        }

        public static string GenerateUrl(this UrlHelper urlHelper, string actionName, string controllerName)
        {

            var urlHelperWithNewContext = new UrlHelper(new RequestContext(urlHelper.RequestContext.HttpContext, new RouteData()), urlHelper.RouteCollection);
            return urlHelperWithNewContext.Action(actionName, controllerName);
        }

        public static string ActionLinkWithParam(this UrlHelper urlHelper, string action, object routeValue)
        {
            var dictionary = new RouteValueDictionary(routeValue);
            var url = urlHelper.Action(action, routeValue);
            if (url == null)
            {
                throw new NullReferenceException("url");
            }
            foreach (var item in dictionary)
            {
                if (item.Value == null)
                {
                    continue;
                }
                url = url.Replace(item.Value.ToString().ToLower(), item.Value.ToString());
            }
            return url;
        }

        public static string ActionLinkWithParam(this UrlHelper urlHelper, string action, string controller, object routeValue)
        {
            var dictionary = new RouteValueDictionary(routeValue);
            var url = urlHelper.Action(action, controller, routeValue);
            if (url == null)
            {
                throw new NullReferenceException("url");
            }
            foreach (var item in dictionary)
            {
                if (item.Value == null)
                {
                    continue;
                }
                url = url.Replace(item.Value.ToString().ToLower(), item.Value.ToString());
            }
            return url;
        }
    }
}