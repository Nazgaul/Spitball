using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    public class BaseController : AsyncController
    {
        protected const string SessionUserUploadProfilePicturesKey = "UserUploadProfilePictures";
        //public  const string TempDataNameUserRegisterFirstTime = "TempDataNameUserRegisterFirstTime";

        protected readonly IZboxWriteService m_ZboxWriteService;
        protected readonly IZboxReadService m_ZboxReadService;
        //protected readonly IShortCodesCache m_ShortToLongCode;
        protected readonly IFormsAuthenticationService m_FormsAuthenticationService;

        public BaseController()
        {
            //error controller only
            m_FormsAuthenticationService = Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<IFormsAuthenticationService>();
        }

        public BaseController(IZboxWriteService zboxWriteService, IZboxReadService zboxReadService,
            IFormsAuthenticationService formsAuthenticationService)
        {
            m_ZboxWriteService = zboxWriteService;
            m_ZboxReadService = zboxReadService;
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
            var parameters = filterContext.HttpContext.Request.Params.ToString().Replace("&", "\n");
            var info = string.Format("url {0} user {1} params {2} ", filterContext.HttpContext.Request.RawUrl, User.Identity.Name, parameters);
            TraceLog.WriteError(info, filterContext.Exception);
            base.OnException(filterContext);
        }
        protected override void HandleUnknownAction(string actionName)
        {
            var parameters = Request.Params.ToString().Replace("&", "\n");
            var info = string.Format("HandleUnknownAction {0} url {1} user {2} params {3} ", actionName, Request.RawUrl, User.Identity.Name, parameters);
            TraceLog.WriteError(info);
            if (Request.IsAjaxRequest())
            {
                this.HttpNotFound().ExecuteResult(this.ControllerContext);
            }
            this.View("Error").ExecuteResult(this.ControllerContext);
            //base.HandleUnknownAction(actionName);
        }

        protected void DeleteCookies()
        {
            var cookiesToDelete = Request.Cookies.AllKeys.Where(w => w.StartsWith("cdAuth") || w.StartsWith("cdA"));
            foreach (var cookieName in cookiesToDelete)
            {
                if (cookieName == FormsAuthentication.FormsCookieName && User.Identity.IsAuthenticated)
                {
                    continue;
                }
                if (Request.Cookies[cookieName] != null)
                {
                    var c = new HttpCookie(cookieName);
                    c.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(c);
                }
            }

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



        protected long GetUserId(bool isAuthorize = true)
        {
            long userId;

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

            TempDataProvider = new CookieTempDataProvider(HttpContext);
            try
            {

                if (User != null && User.Identity.IsAuthenticated)
                {
                    var userData = m_FormsAuthenticationService.GetUserData();
                    if (userData != null)
                    {
                        ChangeThreadLanguage(userData.Language);
                        return;
                    }
                }
                if (Request.QueryString["lang"] != null)
                {
                    ChangeThreadLanguage(Request.QueryString["lang"]);
                    return;
                }
                if (HttpContext.Request.Cookies["lang"] != null)
                {
                    var value = Server.HtmlEncode(HttpContext.Request.Cookies["lang"].Value);
                    ChangeThreadLanguage(value);
                    return;
                }

                if (Request.UserLanguages == null) return;
                foreach (var languageWithRating in Request.UserLanguages)
                {
                    if (string.IsNullOrEmpty(languageWithRating))
                    {
                        continue;
                    }
                    var userLanguage = languageWithRating.Split(';')[0];
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
                    CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
                    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
                    //Thread.CurrentThread.CurrentUICulture = cultureInfo;
                    //Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
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
            return null;
        }
        #endregion
    }
}
