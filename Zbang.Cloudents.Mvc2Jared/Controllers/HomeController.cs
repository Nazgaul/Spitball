using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc2Jared.Models;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Queries.Jared;
using System.Threading;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Mvc2Jared.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class HomeController : BaseController
    {
        IZboxReadService m_readService;
        public HomeController(IZboxReadService readService)
        {
            m_readService = readService;
        }
        // private readonly Lazy<ApplicationUserManager> m_UserManager;
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Page()
        {
            return View();
        }
     
        [HttpPost, ActionName("Items")]
        public async Task<JsonResult> ItemsAsync(JaredSearchQuery model, CancellationToken cancellationToken)
        {
            int a;
            a = 5;
           var retVal = await m_readService.GetItemsWithTagsAsync(model);
            a = 5;
            //return Json(retVal);
            return Json(retVal);
            //return JsonOk();
            //return await SearchAllQueryAsync(cancellationToken,search);
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<JsonResult> LogInAsync(LogOn model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            try
            {
                if (model.Email == "yifatbij@gmail.com" && model.Password == "123123")
                {
                    return JsonOk(Url.Action("Page", "home"));
                }
                //ModelState.AddModelError(string.Empty, AccountValidation.ErrorCodeToString(AccountValidation.AccountError.InvalidPassword));
                return JsonError(new { error="invalid password Or user name"});
                //var query = new GetUserByEmailQuery(model.Email);
                //var tSystemData = ZboxReadService.GetUserDetailsByEmail(query);
                //var tUserIdentity = m_UserManager.Value.FindByEmailAsync(model.Email);

                //await Task.WhenAll(tSystemData, tUserIdentity);

                //var user = tUserIdentity.Result;
                //var systemUser = tSystemData.Result;


                //if (systemUser == null)
                //{
                //    ModelState.AddModelError(string.Empty, AccountControllerResources.LogonError);
                //    return JsonError(GetErrorFromModelState());
                //}

                //if (systemUser.MembershipId.HasValue)
                //{
                //    if (user == null)
                //    {
                //        ModelState.AddModelError(string.Empty, AccountValidation.ErrorCodeToString(AccountValidation.AccountError.InvalidEmail));
                //        return JsonError(GetErrorFromModelState());
                //    }
                //    var loginStatus = await m_UserManager.Value.CheckPasswordAsync(user, model.Password);

                //    if (loginStatus)
                //    {
                //        var identity = await user.GenerateUserIdentityAsync(m_UserManager.Value, systemUser.Id,
                //            systemUser.UniversityId, systemUser.UniversityData);
                //        m_AuthenticationManager.SignIn(new AuthenticationProperties
                //        {
                //            IsPersistent = model.RememberMe,
                //        }, identity);

                //        m_CookieHelper.RemoveCookie(Invite.CookieName);
                //        m_LanguageCookie.InjectCookie(systemUser.Culture);

                //        var url = systemUser.UniversityId.HasValue
                //            ? Url.Action("Index", "Dashboard")
                //            : Url.Action("Choose", "University");
                //        return JsonOk(url);

                //    }
                //    ModelState.AddModelError(string.Empty, AccountValidation.ErrorCodeToString(AccountValidation.AccountError.InvalidPassword));
                //    return JsonError(GetErrorFromModelState());
                //}
                //if (systemUser.FacebookId.HasValue)
                //{
                //    ModelState.AddModelError(string.Empty, AccountControllerResources.RegisterEmailFacebookAccountError);
                //    return JsonError(GetErrorFromModelState());
                //}
                //if (!string.IsNullOrEmpty(systemUser.GoogleId))
                //{
                //    ModelState.AddModelError(string.Empty, AccountControllerResources.RegisterEmailGoogleAccountError);
                //    return JsonError(GetErrorFromModelState());
                //}
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"LogOn model : {model} ", ex);
                ModelState.AddModelError(string.Empty, "Logon Error");
            }
            return JsonError(GetErrorFromModelState());
        }
    }
}