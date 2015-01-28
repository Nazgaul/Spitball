using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Zbang.Cloudents.SiteExtension;

namespace Zbang.Cloudents.Mobile.Filters
{
    public class ValidateAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    ValidateRequestHeader(filterContext.HttpContext);
                }
                else
                {
                    AntiForgery.Validate();
                }
            }
            catch (HttpAntiForgeryException)
            {
                throw new HttpAntiForgeryException("Anti forgery token cookie not found");
            }
        }

        private void ValidateRequestHeader(HttpContextBase httpContent)
        {
            var cookieHelper = new CookieHelper(httpContent);

            var cookieToken = cookieHelper.ReadCookie<string>(AntiForgeryConfig.CookieName);
            string formToken = httpContent.Request.Headers["RequestVerificationToken"];
            AntiForgery.Validate(cookieToken, formToken);
        }
    }
}