using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Zbang.Zbox.Infrastructure.Extensions;


namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class RedirectToMobileAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.Browser.IsMobileDevice) return;
            var urlToRedirect = ConfigFetcher.Fetch("MobileWebSite");
            var url = filterContext.HttpContext.Request.Url;
            var path = string.Empty;
            if (url != null)
            {
                path = url.PathAndQuery;
            }
            urlToRedirect = VirtualPathUtility.RemoveTrailingSlash(urlToRedirect);
            urlToRedirect = urlToRedirect + path;
            filterContext.Result = new RedirectResult(urlToRedirect, true);
        }
    }

    public class RedirectToWWW : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var serverDomain = filterContext.HttpContext.Request.Url.Host;
            if (serverDomain.StartsWith("ono"))
            {
                filterContext.Result = new RedirectResult("https://www.cloudents.com" + filterContext.HttpContext.Request.Url.PathAndQuery, true);
            }
            //     If Not GetServerDomain.StartsWith("www.") Then 
            //    HttpContext.Current.Response.Status = "301 Moved Permanently" 
            //    HttpContext.Current.Response.AddHeader("Location", "http://www." & GetServerDomain() & HttpContext.Current.Request.RawUrl)  
            //End If 
            base.OnActionExecuting(filterContext);
        }

        //public string GetServerDomain(Uri url)
        //{
        //    var myUrl = url.ToString();
        //    var re = new Regex(@"^(?:(?:https?\:)?(?:\/\/)?)?([^\/]+)");
        //    var m = re.Match(myUrl);
        //    return m.Groups[1].Value;
        //}
        //    Public Shared Function GetServerDomain() As String 
        //    Dim myURL As String = HttpContext.Current.Request.Url.ToString  
        //    Dim re As New Regex("^(?:(?:https?\:)?(?:\/\/)?)?([^\/]+)")  
        //    Dim m As Match = re.Match(myURL)  
        //    Return m.Groups(1).Value  
        //End Function    
    }
}