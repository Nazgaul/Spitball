using System.Web.Mvc;
using System.Web.Routing;
using Zbang.Cloudents.Mobile.Helpers;
using Zbang.Cloudents.SiteExtension;

namespace Zbang.Cloudents.Mobile.Filters
{
    public class NoUniversityAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                return;
            }
            var universityId = filterContext.HttpContext.User.GetUniversityId();
            if (universityId.HasValue)
            {
                base.OnActionExecuting(filterContext);
                return;
            }
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult { Data = new JsonResponse(false), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return;
            }
            filterContext.Result = new RedirectToRouteResult("libraryChoose",
               new RouteValueDictionary
               {
                   { "returnUrl",filterContext.HttpContext.Request.QueryString["returnUrl"]},
                   { "new", filterContext.HttpContext.Request.QueryString["new"]}

               });
        }
    }
}