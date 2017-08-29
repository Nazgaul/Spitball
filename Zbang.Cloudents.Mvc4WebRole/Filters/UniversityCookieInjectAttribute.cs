using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models.Account;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    [System.AttributeUsageAttribute(System.AttributeTargets.All, AllowMultiple = false)]
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
            var cookieHelper = CookieHelper.ReadCookie<UniversityCookie>(UniversityCookie.CookieName);
            filterContext.HttpContext.Items[UniversityCookie.CookieName] = cookieHelper;

            if (!string.IsNullOrEmpty(universityName))
            {
                //if we have cookie check if its the same as url
                if (string.Equals(cookieHelper?.UniversityName, universityName,
                    System.StringComparison.InvariantCultureIgnoreCase))
                {
                    base.OnActionExecuting(filterContext);
                    return;
                }

                var universityId = ZboxReadService.GetUniversityIdByUrl(universityName);
                if (!universityId.HasValue)
                {
                    base.OnActionExecuting(filterContext);
                    return;
                    //return RedirectToRoute("homePage", new { invId });
                }
                var universityCookie = new UniversityCookie
                {
                    UniversityId = universityId.Value,
                    UniversityName = universityName
                };

                filterContext.HttpContext.Items[UniversityCookie.CookieName] = universityCookie;
                CookieHelper.InjectCookie(UniversityCookie.CookieName, universityCookie);
            }
            base.OnActionExecuting(filterContext);
        }
    }
}