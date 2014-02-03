using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DevTrends.MvcDonutCaching;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ReadServices;
using System.Threading;
using System.Globalization;
using Zbang.Zbox.Infrastructure.Culture;
using System.Web.Security;
using Zbang.Zbox.Infrastructure.Security;


namespace Zbang.Zbox.Mvc3WebRole.Controllers
{
    public class BaseController : AsyncController
    {
        const string SessionLanguageKey = "userLanguage";
        protected const string SessionUserUploadProfilePicturesKey = "UserUploadProfilePictures";

        protected readonly IZboxWriteService m_ZboxWriteService;
        protected readonly IZboxReadService m_ZboxReadService;
        protected readonly IShortCodesCache m_ShortToLongCode;
        protected readonly IFormsAuthenticationService m_FormsAuthenticationService;

        public BaseController(IZboxWriteService zboxWriteService, IZboxReadService zboxReadService,
           IShortCodesCache shortToLongCache, IFormsAuthenticationService formsAuthenticationService)
        {
            m_ZboxWriteService = zboxWriteService;
            m_ZboxReadService = zboxReadService;
            m_ShortToLongCode = shortToLongCache;
            m_FormsAuthenticationService = formsAuthenticationService;
        }




        protected string RenderRazorViewToString(string viewName, object model)
        {

            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        protected override void OnException(ExceptionContext filterContext)
        {

            var info = string.Format("url {0} user {1} params {2} ", filterContext.HttpContext.Request.RawUrl, User.Identity.Name, filterContext.HttpContext.Request.Params);
            TraceLog.WriteError(info, filterContext.Exception);
            base.OnException(filterContext);
        }

        protected IEnumerable<KeyValuePair<string, string[]>> GetModelStateErrors()
        {
            return ModelState.ToDictionary(kvp => kvp.Key,
                 kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()).Where(m => m.Value.Any());
        }

        protected IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
        }

        protected UserPermissionPerBoxDto GetUserPermission(long userid, long boxid)
        {

            var query = new GetBoxQuery(boxid, userid);
            var permission = m_ZboxReadService.GetUserPermission(query);
            return permission;
        }

        protected long GetUserId(bool isAuthorize = true)
        {
            long userId = -1;

            if (isAuthorize && string.IsNullOrEmpty(User.Identity.Name))
            {
                throw new UnauthorizedAccessException();
            }
            long.TryParse(User.Identity.Name, out userId);

            return userId;
        }

        #region Language
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            try
            {
                var language = Languages.GetDefaultSystemCulture().Name;
                if (User.Identity.IsAuthenticated)
                {
                    var userData = m_FormsAuthenticationService.GetUserData();
                    if (userData != null)
                    {
                        ChangeThreadLanguage(userData.Language);
                        return;
                    }
                }
                var queryStringLanguage = GetQueryStringLanguage(requestContext.HttpContext.Request.QueryString);
                if (!string.IsNullOrEmpty(queryStringLanguage))
                {
                    ChangeThreadLanguage(queryStringLanguage);
                    return;
                }
                foreach (var languageWithRating in Request.UserLanguages)
                {
                    string userLanguage = languageWithRating.Split(';')[0];
                    if (Languages.CheckIfLanguageIsSupported(userLanguage))
                    {
                        ChangeThreadLanguage(userLanguage);
                        break;
                    }
                }
               
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("initialize", ex);
            }
        }
        protected void ChangeThreadLanguage(string language)
        {
            if (!Languages.CheckIfLanguageIsSupported(language))
            {
                return;
            }
            if (Thread.CurrentThread.CurrentUICulture.Name != language)
            {
                try
                {
                    CultureInfo cultureInfo = new CultureInfo(language);
                    Thread.CurrentThread.CurrentUICulture = cultureInfo;
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
                }
                catch (CultureNotFoundException)
                {
                    // RemoveLanguageFromSession();
                }
            }
        }

        private string GetQueryStringLanguage(System.Collections.Specialized.NameValueCollection nameValueCollection)
        {
            //object queryStringLanguage;
            object queryStringLanguage = nameValueCollection["language"];
            //filterContext.ActionParameters.TryGetValue("language", out queryStringLanguage);

            if (queryStringLanguage != null && Languages.CheckIfLanguageIsSupported(queryStringLanguage.ToString()))
            {
                return queryStringLanguage.ToString();
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
