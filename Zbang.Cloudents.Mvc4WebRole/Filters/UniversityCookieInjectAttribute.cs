using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models.Account;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class UniversityCookieInjectAttribute : ActionFilterAttribute
    {
        private readonly string m_UniversityPrefix;

        public UniversityCookieInjectAttribute(string universityPrefix)
        {
            m_UniversityPrefix = universityPrefix;
        }

        public IZboxReadService ZboxReadService { get; set; }
        public ICookieHelper CookieHelper { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //CookieHelper.RemoveCookie(UniversityCookie.CookieName);
            var universityName = filterContext.ActionParameters[m_UniversityPrefix]?.ToString();


            if (!string.IsNullOrEmpty(universityName))
            {
                var universityId = ZboxReadService.GetUniversityIdByUrl(universityName);
                if (!universityId.HasValue)
                {
                    base.OnActionExecuting(filterContext);
                    return;
                    //return RedirectToRoute("homePage", new { invId });
                }
                CookieHelper.InjectCookie(UniversityCookie.CookieName, 
                    new UniversityCookie { UniversityId = universityId.Value, UniversityName = universityName });
            }
            base.OnActionExecuting(filterContext);
        }
    }
}