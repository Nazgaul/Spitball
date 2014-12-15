using System;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class PreserveQueryStringAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var redirectResult = filterContext.Result as RedirectToRouteResult;



            if (redirectResult != null)
            {
                var query = filterContext.HttpContext.Request.QueryString;
                // Remark: here you could decide if you want to propagate all
                // query string values or a particular one. In my example I am
                // propagating all query string values that are not already part of
                // the route values
                foreach (string key in query.Keys)
                {
                    if (!redirectResult.RouteValues.ContainsKey(key))
                    {
                        redirectResult.RouteValues.Add(key, query[key]);
                    }
                }
                return;
            }
            var redirectResult2 = filterContext.Result as RedirectResult;
            if (redirectResult2 != null && filterContext.HttpContext.Request.Url != null)
            {
                //var query = filterContext.HttpContext.Request.Url.Query.QueryString;
                var uri = new UriBuilder(redirectResult2.Url) { Query = filterContext.HttpContext.Request.Url.Query };
                redirectResult2 = new RedirectResult(uri.ToString(), redirectResult2.Permanent);
                filterContext.Result = redirectResult2;
            }


        }
    }
}