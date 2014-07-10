using Microsoft.Practices.Unity;
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
    public class BaseController : Controller
    {
        protected const string SessionUserUploadProfilePicturesKey = "UserUploadProfilePictures";

        [Dependency]
        protected IZboxWriteService ZboxWriteService {get;set;}
        [Dependency]
        protected IZboxReadService ZboxReadService { get; set; }
        [Dependency]
        protected IFormsAuthenticationService FormsAuthenticationService { get; set; }

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
            var info = string.Format("HandleUnknownAction {0} url {1} user {2} isAjax {4} params {3} "
                , actionName, Request.RawUrl, User.Identity.Name, parameters, Request.IsAjaxRequest());
            TraceLog.WriteError(info);
            if (Request.IsAjaxRequest())
            {
                HttpNotFound().ExecuteResult(ControllerContext);
            }
            View("Error").ExecuteResult(ControllerContext);
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
                if (Request.Cookies[cookieName] == null) continue;
                var c = new HttpCookie(cookieName) { Expires = DateTime.Now.AddDays(-1) };
                Response.Cookies.Add(c);
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
                    var userData = FormsAuthenticationService.GetUserData();
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
                var langFromUrl =  requestContext.RouteData.Values.FirstOrDefault(f => f.Key == "lang");
                if (langFromUrl.Value != null)
                {
                    ChangeThreadLanguage(langFromUrl.Value.ToString());
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
            if (Thread.CurrentThread.CurrentUICulture.Name == language) return;
            try
            {
                var cultureInfo = new CultureInfo(language);
                //CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
                //CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
            }
            catch (CultureNotFoundException)
            {
                // RemoveLanguageFromSession();
            }
        }
      
        #endregion
    }
}
