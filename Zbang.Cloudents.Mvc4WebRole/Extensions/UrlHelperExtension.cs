using System;
using System.Linq;
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

        //public static string ActionHash(this UrlHelper urlHelper, string action, object routeValues, string hash)
        //{
        //    return $"{urlHelper.Action(action, routeValues)}#{hash}";
        //}
        //public static string ActionHash(this UrlHelper urlHelper, string action, string controller, string hash)
        //{
        //    return $"{urlHelper.Action(action, controller)}#{hash}";
        //}

        //public static string GenerateUrl(this UrlHelper urlHelper, string actionName, string controllerName)
        //{

        //    var urlHelperWithNewContext = new UrlHelper(new RequestContext(urlHelper.RequestContext.HttpContext, new RouteData()), urlHelper.RouteCollection);
        //    return urlHelperWithNewContext.Action(actionName, controllerName);
        //}

        public static string RouteUrlCache(this UrlHelper urlHelper, string routeName,
            RouteValueDictionary routeValues)
        {
            var cache = urlHelper.RequestContext.HttpContext.Cache;
            var key = routeName + string.Join("!", routeValues.Values.Select(x => x.ToString()));
            var retVal = cache[key] as string;
            if (retVal != null) return retVal;
            retVal = urlHelper.RouteUrl(routeName, routeValues);
            //foreach (var item in routeValues)
            //{
            //    if (item.Value == null)
            //    {
            //        continue;
            //    }
            //    retVal = retVal.Replace(item.Value.ToString().ToLower(), item.Value.ToString());
            //}
            cache.Add(key, retVal, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromDays(30),
                System.Web.Caching.CacheItemPriority.Default, null);
            cache[key] = retVal;
            return retVal;
        }
        


        //public static string ActionLinkWithParam(this UrlHelper urlHelper, string action, object routeValue)
        //{
        //    var dictionary = new RouteValueDictionary(routeValue);
        //    var url = urlHelper.Action(action, routeValue);
        //    if (url == null)
        //    {
        //        throw new NullReferenceException("url");
        //    }
        //    foreach (var item in dictionary)
        //    {
        //        if (item.Value == null)
        //        {
        //            continue;
        //        }
        //        url = url.Replace(item.Value.ToString().ToLower(), item.Value.ToString());
        //    }
        //    return url;
        //}
        

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