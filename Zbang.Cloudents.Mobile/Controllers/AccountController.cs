using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mobile.Controllers.Resources;
using Zbang.Cloudents.Mobile.Extensions;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Cloudents.Mobile.Helpers;
using Zbang.Cloudents.Mobile.Models.Account;
using Zbang.Cloudents.Mobile.Models.Account.Settings;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ViewModel.Dto.UserDtos;
using Zbang.Zbox.ViewModel.Queries;


namespace Zbang.Cloudents.Mobile.Controllers
{

    public class AccountController : BaseController
    {
        private readonly Lazy<IMembershipService> m_MembershipService;
        private readonly Lazy<IFacebookService> m_FacebookService;
        private readonly Lazy<IQueueProvider> m_QueueProvider;
        private readonly Lazy<IEncryptObject> m_EncryptObject;

        // private const string InvId = "invId";
        public AccountController(
            Lazy<IMembershipService> membershipService,
            Lazy<IFacebookService> facebookService,
            Lazy<IQueueProvider> queueProvider,
            Lazy<IEncryptObject> encryptObject
            )
        {

            m_MembershipService = membershipService;
            m_FacebookService = facebookService;
            m_QueueProvider = queueProvider;
            m_EncryptObject = encryptObject;
        }


        [HttpGet]
        [DonutOutputCache(CacheProfile = "PartialPage",
           Options = OutputCacheOptions.IgnoreQueryString
           )]
        public ActionResult IndexPartial()
        {
            return PartialView("Index2");
        }

        [HttpGet]
        [DonutOutputCache(CacheProfile = "PartialPage",
           Options = OutputCacheOptions.IgnoreQueryString
           )]
        public ActionResult LoginPartial()
        {
            return PartialView("_Login");
        }

        [HttpGet]
        [DonutOutputCache(CacheProfile = "PartialPage",
           Options = OutputCacheOptions.IgnoreQueryString
           )]
        public ActionResult RegisterPartial()
        {
            return PartialView("_Register");
        }



        #region Login
        [HttpPost]
        public async Task<JsonResult> FacebookLogin(string token,
            long? universityId)
        {
            try
            {
                var cookie = new CookieHelper(HttpContext);
                var facebookUserData = await m_FacebookService.Value.FacebookLogIn(token);
                if (facebookUserData == null)
                {
                    return JsonError(new { error = AccountControllerResources.FacebookGetDataError });
                }
                var query = new GetUserByFacebookQuery(facebookUserData.id);
                LogInUserDto user = await ZboxReadService.GetUserDetailsByFacebookId(query);
                if (user == null)
                {

                    var inv = cookie.ReadCookie<Invite>(Invite.CookieName);
                    Guid? invId = null;
                    if (inv != null)
                    {
                        invId = inv.InviteId;
                    }
                    var command = new CreateFacebookUserCommand(facebookUserData.id, facebookUserData.email,
                        facebookUserData.Image, facebookUserData.LargeImage, universityId,
                        facebookUserData.first_name,
                        facebookUserData.middle_name,
                        facebookUserData.last_name,
                        facebookUserData.GetGender(),
                        false, facebookUserData.locale, invId, null, true);
                    var commandResult = await ZboxWriteService.CreateUserAsync(command);
                    user = new LogInUserDto
                    {
                        Id = commandResult.User.Id,
                        Culture = commandResult.User.Culture,
                        Image = facebookUserData.Image,
                        Name = facebookUserData.name,
                        UniversityId = commandResult.UniversityId,
                        UniversityData = commandResult.UniversityData,
                        Score = commandResult.User.Reputation
                    };
                }
                cookie.InjectCookie(Helpers.UserLanguage.CookieName, new Language { Lang = user.Culture });
                FormsAuthenticationService.SignIn(user.Id, false, new UserDetail(
                    //user.Culture,
                    user.UniversityId,
                    user.UniversityData
                    ));
                cookie.RemoveCookie(Invite.CookieName);
                return JsonOk();
            }
            catch (ArgumentException)
            {
                return JsonError(new { error = AccountControllerResources.FacebookGetDataError });
            }
            catch (NullReferenceException ex)
            {
                TraceLog.WriteError("token: " + token, ex);
                return JsonError(new { error = AccountControllerResources.FacebookGetDataError });
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("FacebookLogin", ex);
                return JsonError(new { error = AccountControllerResources.FacebookGetDataError });
            }
        }




        //[ValidateAntiForgeryToken]
        [HttpPost]
        [Filters.ValidateAntiForgeryToken]
        public async Task<JsonResult> Login([ModelBinder(typeof(TrimModelBinder))]LogOn model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetModelStateErrors());
            }
            try
            {
                Guid membershipUserId;
                var cookie = new CookieHelper(HttpContext);
                cookie.RemoveCookie(Invite.CookieName);
                var loginStatus = m_MembershipService.Value
                    .ValidateUser(model.Email, model.Password, out membershipUserId);
                if (loginStatus == LogInStatus.Success)
                {
                    try
                    {
                        var query = new GetUserByMembershipQuery(membershipUserId);
                        var result = await ZboxReadService.GetUserDetailsByMembershipId(query);
                        cookie.InjectCookie(Helpers.UserLanguage.CookieName, new Language { Lang = result.Culture });
                        FormsAuthenticationService.SignIn(result.Id, model.RememberMe,
                            new UserDetail(
                            //result.Culture,
                                result.UniversityId,
                                result.UniversityData));
                        return JsonOk();
                    }
                    catch (UserNotFoundException)
                    {
                        ModelState.AddModelError(string.Empty, AccountControllerResources.LogonError);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, loginStatus.GetEnumDescription());
                }

            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("LogOn model : {0} ", model), ex);
                ModelState.AddModelError(string.Empty, AccountControllerResources.LogonError);
            }
            return JsonError(GetModelStateErrors());
        }



        //TODO: do a post on log out
        [RemoveBoxCookie]
        public ActionResult LogOff()
        {

            Session.Abandon(); // remove the session cookie from user computer. wont continue session if user log in with a diffrent id.            
            FormsAuthenticationService.SignOut();
            return Redirect(FormsAuthentication.LoginUrl.ToLower());// RedirectToAction("Index");
        }



        [HttpPost]
        public async Task<JsonResult> Register([ModelBinder(typeof(TrimModelBinder))] Register model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetModelStateErrors());
            }
            var userid = Guid.NewGuid().ToString();
            try
            {
                var cookie = new CookieHelper(HttpContext);
                var inv = cookie.ReadCookie<Invite>(Invite.CookieName);
                Guid userProviderKey;
                var createStatus = m_MembershipService.Value.CreateUser(userid, model.Password, model.NewEmail,
                    out userProviderKey);
                if (createStatus == MembershipCreateStatus.Success)
                {

                    Guid? invId = null;
                    if (inv != null)
                    {
                        invId = inv.InviteId;
                    }

                    CreateUserCommand command = new CreateMembershipUserCommand(userProviderKey,
                        model.NewEmail, model.UniversityId, model.FirstName, model.LastName,
                        !model.IsMale.HasValue || model.IsMale.Value,
                        model.MarketEmail, CultureInfo.CurrentCulture.Name, invId, null, true);
                    var result = await ZboxWriteService.CreateUserAsync(command);
                    cookie.InjectCookie(Helpers.UserLanguage.CookieName, new Language { Lang = result.User.Culture });
                    FormsAuthenticationService.SignIn(result.User.Id, false,
                        new UserDetail(
                        //result.User.Culture,
                            result.UniversityId,
                            result.UniversityData));
                    cookie.RemoveCookie(Invite.CookieName);
                    return
                        JsonOk();

                }
                ModelState.AddModelError(string.Empty, AccountValidation.ErrorCodeToString(createStatus));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong");
                TraceLog.WriteError("Register model:" + model, ex);
            }
            return JsonError(GetModelStateErrors());
        }
        #endregion


        [HttpPost, ZboxAuthorize]
        public async Task<ActionResult> UpdateUniversity(University model)
        {
            var retVal = await ZboxReadService.GetRussianDepartmentList(model.UniversityId);
            if (retVal.Count() != 0 /*&& !model.DepartmentId.HasValue*/)
            {
                return JsonError("select department");
                //return RedirectToAction("SelectDepartment", "Library", new { universityId = model.UniversityId });
            }
            var needId = await ZboxReadService.GetUniversityNeedId(model.UniversityId);
            if (needId /*&& string.IsNullOrEmpty(model.StudentId)*/)
            {
                return JsonError("insert id");
                //return RedirectToAction("InsertId", "Library", new { universityId = model.UniversityId });
            }

            var needCode = await ZboxReadService.GetUniversityNeedCode(model.UniversityId);
            if (needCode /*&& string.IsNullOrEmpty(model.Code)*/)
            {
                return JsonError("insert code");
                //return RedirectToAction("InsertCode", "Library", new { universityId = model.UniversityId });
            }

            if (!ModelState.IsValid)
            {
                return JsonError(new { error = GetModelStateErrors() });
            }

            try
            {
                var id = User.GetUserId();
                var command = new UpdateUserUniversityCommand(model.UniversityId, id, null, null,
                    null, null, null);
                ZboxWriteService.UpdateUserUniversity(command);
                FormsAuthenticationService.ChangeUniversity(command.UniversityId, command.UniversityDataId);
                return JsonOk();
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError("Code", Models.Account.Resources.AccountSettingsResources.CodeIncorrect);
                return JsonError(GetModelStateErrors());

            }
            catch (NullReferenceException)
            {
                ModelState.AddModelError("Code", Models.Account.Resources.AccountSettingsResources.CodeIncorrect);
                return JsonError(GetModelStateErrors());

            }
            catch (Exception ex)
            {
                TraceLog.WriteError("update university model " + model, ex);
                ModelState.AddModelError("Code", Models.Account.Resources.AccountSettingsResources.CodeIncorrect);
                return JsonError(GetModelStateErrors());
            }
        }



        [HttpPost]
        public ActionResult ChangeLocale(string lang)
        {
            var cookie = new CookieHelper(HttpContext);
            cookie.InjectCookie(Helpers.UserLanguage.CookieName, new Language { Lang = lang });
            return JsonOk();
        }


        //#endregion
        #region passwordReset
        private const string SessionResetPassword = "SResetPassword";
        private const string ResetPasswordCrypticPropose = "reset password";
        [HttpGet]
        public ActionResult ResetPassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("dashboardLink");
            }
            return View("ForgotPwd");
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword([ModelBinder(typeof(TrimModelBinder))]ForgotPassword model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("dashboardLink");
            }
            if (!ModelState.IsValid)
            {
                return View("ForgotPwd", model);
            }
            try
            {
                Guid membershipUserId;
                if (!m_MembershipService.Value.EmailExists(model.Email, out membershipUserId))
                {
                    ModelState.AddModelError("Email", AccountControllerResources.EmailDoesNotExists);
                    return View("ForgotPwd", model);
                }
                var query = new GetUserByMembershipQuery(membershipUserId);
                var result = await ZboxReadService.GetUserDetailsByMembershipId(query);
                var code = RandomString(10);


                var data = new ForgotPasswordLinkData(membershipUserId, 1);

                var linkData = CrypticElement(data);
                Session[SessionResetPassword] = data;
                await m_QueueProvider.Value.InsertMessageToMailNewAsync(new ForgotPasswordData2(code, linkData, result.Name.Split(' ')[0], model.Email, result.Culture));

                TempData["key"] = System.Web.Helpers.Crypto.HashPassword(code);

                return RedirectToAction("Confirmation");

            }

            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("ForgotPassword email: {0}", model.Email), ex);
                ModelState.AddModelError(string.Empty, AccountControllerResources.UnspecifiedError);
            }



            return View("ForgotPwd", model);
        }

        [HttpGet, NoCache]
        public ActionResult Confirmation()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("dashboardLink");
            }
            if (TempData["key"] == null)
            {
                return RedirectToAction("ResetPassword");
            }
            var userData = Session[SessionResetPassword] as ForgotPasswordLinkData;
            if (userData == null)
            {
                return RedirectToAction("ResetPassword");
            }
            return View("CheckEmail", new Confirmation { Key = TempData["key"].ToString() });
        }

        [HttpPost]
        public ActionResult Confirmation([ModelBinder(typeof(TrimModelBinder))] Confirmation model)
        {
            if (!ModelState.IsValid)
            {
                return View("CheckEmail", model);
            }
            var userData = Session[SessionResetPassword] as ForgotPasswordLinkData;

            if (userData == null)
            {
                return RedirectToAction("ResetPassword");
            }
            if (System.Web.Helpers.Crypto.VerifyHashedPassword(model.Key, model.Code))
            {
                userData.Step = 2;
                var key = CrypticElement(userData);
                return RedirectToAction("PasswordUpdate", new { key });
            }
            ModelState.AddModelError(string.Empty, "This is not the correct code");
            return View("CheckEmail", model);
        }

        [HttpGet]
        public ActionResult PasswordUpdate(string key)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("dashboardLink");
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                return RedirectToRoute("accountLink");

            }
            var data = UnEncryptElement<ForgotPasswordLinkData>(key);
            if (data == null)
            {
                return RedirectToRoute("accountLink");
            }
            if (data.Date.AddHours(1) < DateTime.UtcNow)
            {
                return RedirectToRoute("accountLink");
            }
            return View("ChoosePwd", new NewPassword());
        }

        [HttpPost]
        public async Task<ActionResult> PasswordUpdate([ModelBinder(typeof(TrimModelBinder))] NewPassword model, string key)
        {
            if (!ModelState.IsValid)
            {
                return View("ChoosePwd", model);
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                return View("ChoosePwd", model);
            }
            var data = UnEncryptElement<ForgotPasswordLinkData>(key);
            if (data == null)
            {
                return RedirectToRoute("accountLink");
            }
            if (data.Date.AddHours(1) < DateTime.UtcNow)
            {
                return RedirectToRoute("accountLink");
            }
            m_MembershipService.Value.ResetPassword(data.MembershipUserId, model.Password);

            var query = new GetUserByMembershipQuery(data.MembershipUserId);
            var result = await ZboxReadService.GetUserDetailsByMembershipId(query);
            Session.Abandon();
            var cookie = new CookieHelper(HttpContext);
            cookie.InjectCookie(Helpers.UserLanguage.CookieName, new Language { Lang = result.Culture });
            FormsAuthenticationService.SignIn(result.Id, false,
                new UserDetail(
                //result.Culture,
                    result.UniversityId,
                    result.UniversityData
                    ));

            return RedirectToRoute("dashboardLink");

        }


        [NonAction]
        private string RandomString(int size)
        {
            var random = new Random();
            const string input = "0123456789";
            var chars = Enumerable.Range(0, size)
                                   .Select(x => input[random.Next(0, input.Length)]);
            return new string(chars.ToArray());
            //return "12345";
        }
        [NonAction]
        private string CrypticElement<T>(T obj) where T : class
        {
            return m_EncryptObject.Value.EncryptElement(obj, ResetPasswordCrypticPropose);

        }
        [NonAction]
        private T UnEncryptElement<T>(string str) where T : class
        {
            return m_EncryptObject.Value.DecryptElement<T>(str, ResetPasswordCrypticPropose);

        }
        #endregion



        //[ChildActionOnly]
        //public ActionResult GetUserDetail3()
        //{
        //    const string userDetailView = "_UserDetail4";
        //    if (User == null || !(User.Identity.IsAuthenticated))
        //    {
        //        return PartialView(userDetailView);
        //    }
        //    try
        //    {
        //        var retVal = ZboxReadService.GetUserData(new GetUserDetailsQuery(User.GetUserId()));
        //        //  var userData = m_UserProfile.Value.GetUserData(ControllerContext);
        //        var serializer = new JsonNetSerializer();
        //        var jsonRetVal = serializer.Serialize(retVal);
        //        ViewBag.userDetail = jsonRetVal;
        //        return PartialView(userDetailView, retVal);
        //    }
        //    catch (UserNotFoundException)
        //    {
        //        return new EmptyResult();
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("GetUserDetail user" + User.Identity.Name, ex);
        //        return new EmptyResult();
        //    }
        //}

        [HttpGet]
        public ActionResult Details()
        {
            string cookieToken, formToken;
            AntiForgery.GetTokens(null, out cookieToken, out formToken);
            var token = formToken;

            var cookieHelper = new CookieHelper(HttpContext);
            cookieHelper.InjectCookie(AntiForgeryConfig.CookieName, cookieToken);
            if (!User.Identity.IsAuthenticated)
            {
                return JsonOk(new { token});
            }
            var retVal = ZboxReadService.GetUserData(new GetUserDetailsQuery(User.GetUserId()));
            return JsonOk(new
            {
                retVal.Id,
                retVal.UniversityId,
                retVal.Name,
                retVal.Image,
                retVal.IsAdmin,
                retVal.FirstTimeDashboard,
                retVal.Score,
                retVal.UniversityCountry,
                retVal.UniversityName,
                token
            });

        }


        //[HttpPost, ZboxAuthorize]
        //public JsonResult FirstTime(FirstTime firstTime)
        //{
        //    var userid = User.GetUserId();
        //    var command = new UpdateUserFirstTimeStatusCommand(firstTime, userid);
        //    ZboxWriteService.UpdateUserFirstTimeStatus(command);

        //    return JsonOk();
        //}

        //[HttpGet]
        //[OutputCache(CacheProfile = "PartialCache")]
        //public ActionResult CongratsPartial()
        //{
        //    try
        //    {
        //        return PartialView("_Congrats");
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("_Congrats", ex);
        //        return Json(new JsonResponse(false));
        //    }
        //}

        //[HttpGet]
        //[OutputCache(CacheProfile = "PartialCache")]
        //public ActionResult WelcomeAngularPartial()
        //{
        //    try
        //    {
        //        return PartialView("_WelcomeAngular");
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("_WelcomeAngular", ex);
        //        return Json(new JsonResponse(false));
        //    }
        //}

    }


}
