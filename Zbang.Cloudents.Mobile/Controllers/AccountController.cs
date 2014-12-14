using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mobile.Controllers.Resources;
using Zbang.Cloudents.Mobile.Helpers;
using Zbang.Cloudents.Mobile.Models.Account;
using Zbang.Cloudents.Mvc4WebRole.Controllers;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models.Account;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Profile;
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
            Lazy<IEncryptObject> encryptObject)
        {
            m_MembershipService = membershipService;
            m_FacebookService = facebookService;
            m_QueueProvider = queueProvider;
            m_EncryptObject = encryptObject;
        }


        //[FlushHeader(PartialViewName = "_HomeHeader")]
        //issue with ie
        [DonutOutputCache(VaryByParam = "lang", VaryByCustom = CustomCacheKeys.Auth + ";"
            + CustomCacheKeys.Lang,
            Duration = TimeConsts.Minute * 5,
            Location = OutputCacheLocation.Server
            )]
        [PreserveQueryString]
        public ActionResult Index(string lang, string invId)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            if (!string.IsNullOrEmpty(invId))
            {
                var guid = GuidEncoder.TryParseNullableGuid(invId);
                if (guid.HasValue)
                {
                    var h = new CookieHelper(HttpContext);
                    h.InjectCookie(Invite.CookieName, new Invite { InviteId = guid.Value });
                }
            }
            if (lang != null && lang != Thread.CurrentThread.CurrentUICulture.Name)
            {
                RouteData.Values.Remove("lang");
                return RedirectToAction("Index");
            }
            return View("Index2");
        }


        //issue with ie
        //[DonutOutputCache(VaryByParam = "none", VaryByCustom = CustomCacheKeys.Auth + ";"
        //    + CustomCacheKeys.Lang + ";"
        //    + CustomCacheKeys.Mobile, Duration = TimeConsts.Hour)]
        //[CacheFilter(Duration = 0)]
        //[CompressFilter]
        public ActionResult Welcome(string universityId)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View(new LogOnRegister { LogOn = new LogOn(), Register = new Register() });
        }




        #region Login
        [HttpPost]
        [RequireHttps]
        public async Task<JsonResult> FacebookLogin(string token, long? universityId, string returnUrl, long? boxId)
        {
            try
            {
                var cookie = new CookieHelper(HttpContext);
                LogInUserDto user;
                var isNew = false;
                var facebookUserData = await m_FacebookService.Value.FacebookLogIn(token);
                if (facebookUserData == null)
                {
                    return JsonError(new { error = AccountControllerResources.FacebookGetDataError });
                }
                try
                {
                    var query = new GetUserByFacebookQuery(facebookUserData.id);
                    user = await ZboxReadService.GetUserDetailsByFacebookId(query);
                }
                catch (UserNotFoundException)
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
                        false, facebookUserData.locale, invId, boxId);
                    var commandResult = ZboxWriteService.CreateUser(command);
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
                    isNew = true;
                }

                FormsAuthenticationService.SignIn(user.Id, false, new UserDetail(
                    user.Culture,
                    user.UniversityId,
                    user.UniversityData
                    ));
                //TODO: bring it back
                // TempData[UserProfile.UserDetail] = new UserDetailDto(user);
                cookie.RemoveCookie(Invite.CookieName);
                return JsonOk(new { isnew = isNew, url = Url.Action("Index", "Library", new { returnUrl = CheckIfToLocal(returnUrl), @new = "true" }) });
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



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> LogIn([ModelBinder(typeof(TrimModelBinder))]LogOn model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetModelStateErrors());
            }
            try
            {
                Guid membershipUserId;
                var loginStatus = m_MembershipService.Value.ValidateUser(model.Email, model.Password, out membershipUserId);
                if (loginStatus == LogInStatus.Success)
                {
                    try
                    {
                        var query = new GetUserByMembershipQuery(membershipUserId);
                        var result = await ZboxReadService.GetUserDetailsByMembershipId(query);

                        FormsAuthenticationService.SignIn(result.Id, model.RememberMe,
                            new UserDetail(
                                result.Culture,
                                result.UniversityId,
                                result.UniversityData));
                        // TempData[UserProfile.UserDetail] = new UserDetailDto(result);
                        var url = result.UniversityId.HasValue ? Url.Action("Index", "Dashboard") : Url.Action("Choose", "Library");
                        return JsonOk(url);
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
                var cookie = new CookieHelper(HttpContext);
                cookie.RemoveCookie(Invite.CookieName);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("LogOn model : {0} ", model), ex);
                ModelState.AddModelError(string.Empty, AccountControllerResources.LogonError);
            }
            return JsonError(GetModelStateErrors());
        }


        private string CheckIfToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return returnUrl;
            }
            return string.Empty;
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
        [ValidateAntiForgeryToken]
        public JsonResult Register([ModelBinder(typeof(TrimModelBinder))] Register model)
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
                        model.MarketEmail, model.Language.Language, invId, model.BoxId);
                    var result = ZboxWriteService.CreateUser(command);

                    FormsAuthenticationService.SignIn(result.User.Id, false,
                        new UserDetail(
                            result.User.Culture,
                            result.UniversityId,
                            result.UniversityData));
                    cookie.RemoveCookie(Invite.CookieName);
                    return
                        JsonOk(
                            Url.Action("Index", "Library", new { returnUrl = CheckIfToLocal(model.ReturnUrl), @new = "true" }));

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

        #region AccountSettings
        [ZboxAuthorize, NoUniversity]
        [NoCache]
        public ActionResult Settings()
        {
            var userId = User.GetUserId();
            var query = new GetUserDetailsQuery(userId);

            var user = ZboxReadService.GetUserAccountDetails(query);
            if (Request.IsAjaxRequest())
            {
                return PartialView(user);
            }
            return View(user);
        }

        [ZboxAuthorize, NoUniversity]
        [NoCache]
        public ActionResult SettingsDesktop()
        {
            return View("Empty");
        }

        [ZboxAuthorize, NoUniversity]
        public JsonResult SettingsData()
        {
            var userId = User.GetUserId();
            var query = new GetUserDetailsQuery(userId);

            var user = ZboxReadService.GetUserAccountDetails(query);
            return JsonOk(user);
        }

        [DonutOutputCache(Duration = TimeConsts.Minute * 5,
           Location = OutputCacheLocation.ServerAndClient,
           VaryByCustom = CustomCacheKeys.Lang, Options = OutputCacheOptions.IgnoreQueryString, VaryByParam = "none")]
        [ZboxAuthorize, NoUniversity]
        public PartialViewResult SettingPartial()
        {
            return PartialView("Settings");
        }

        const string SessionKey = "UserVerificationCode";
        [HttpPost]
        [ZboxAuthorize]
        public JsonResult EnterCode(long? code)
        {
            if (!code.HasValue)
            {
                return JsonError(AccountControllerResources.ChangeEmailCodeError);
            }
            var model = Session[SessionKey] as ChangeMail;
            if (model == null)
            {
                return JsonError(AccountControllerResources.ChangeEmailCodeError);
            }
            if (code != model.Code)
            {
                return JsonError(AccountControllerResources.ChangeEmailCodeError);
            }
            var id = User.GetUserId();
            try
            {
                var command = new UpdateUserEmailCommand(id, model.Email);
                ZboxWriteService.UpdateUserEmail(command);
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }

            Session.Remove(SessionKey);
            return JsonOk(model.Email);
        }

        [HttpPost]
        [ZboxAuthorize]
        public JsonResult ChangeEmail(ChangeMail model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetModelStateErrors());
            }
            try
            {
                var rand = new Random();
                var generatedCode = rand.Next(10000, 99999);
                model.Code = generatedCode;
                Session[SessionKey] = model;

                m_QueueProvider.Value.InsertMessageToMailNew(new ChangeEmailData(generatedCode.ToString(CultureInfo.InvariantCulture),
                    model.Email, Thread.CurrentThread.CurrentCulture.Name));
                return JsonOk(new { code = true });
            }
            catch (ArgumentException ex)
            {
                return JsonError(new { error = ex.Message });
            }
        }

        [HttpPost]
        [ZboxAuthorize]
        public ActionResult ChangeProfile([ModelBinder(typeof(TrimModelBinder))]Profile model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return JsonError(new { error = GetModelStateErrors() });
                }
                var id = User.GetUserId();
                var profilePics = new ProfileImages(model.Image, model.LargeImage);

                var command = new UpdateUserProfileCommand(id, profilePics.Image,
                    profilePics.LargeImage, model.FirstName, model.MiddleName, model.LastName);
                ZboxWriteService.UpdateUserProfile(command);
                return JsonOk();
            }
            catch (UserNotFoundException)
            {
                return JsonError(new { error = "User doesn't exists" });
            }
        }
        [HttpPost, ZboxAuthorize]
        public async Task<ActionResult> UpdateUniversity(University model)
        {
            var retVal = await ZboxReadService.GetRussianDepartmentList(model.UniversityId);
            if (retVal.Count() != 0 && !model.DepartmentId.HasValue)
            {
                return RedirectToAction("SelectDepartment", "Library", new { universityId = model.UniversityId });
            }
            var needId = await ZboxReadService.GetUniversityNeedId(model.UniversityId);
            if (needId && string.IsNullOrEmpty(model.studentID))
            {
                return RedirectToAction("InsertId", "Library", new { universityId = model.UniversityId });
            }

            var needCode = await ZboxReadService.GetUniversityNeedCode(model.UniversityId);
            if (needCode && string.IsNullOrEmpty(model.Code))
            {
                return RedirectToAction("InsertCode", "Library", new { universityId = model.UniversityId });
            }

            if (!ModelState.IsValid)
            {
                return JsonError(new { error = GetModelStateErrors() });
            }

            try
            {
                var id = User.GetUserId();
                var command = new UpdateUserUniversityCommand(model.UniversityId, id, model.DepartmentId, model.Code,
                    model.GroupNumber, model.RegisterNumber, model.studentID);
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
        [ZboxAuthorize]
        public ActionResult ChangePassword(Password model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetModelStateErrors());
            }
            var id = User.GetUserId();
            var command = new UpdateUserPasswordCommand(id, model.CurrentPassword, model.NewPassword);
            var commandResult = ZboxWriteService.UpdateUserPassword(command);
            return Json(new JsonResponse(!commandResult.HasErrors, commandResult.Error));
        }


        [HttpPost]
        [ZboxAuthorize]
        [RequireHttps]
        public ActionResult ChangeLanguage(Mvc4WebRole.Models.Account.Settings.UserLanguage model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetModelStateErrors());
            }
            var id = User.GetUserId();
            var command = new UpdateUserLanguageCommand(id, model.Language);
            ZboxWriteService.UpdateUserLanguage(command);
            FormsAuthenticationService.ChangeLanguage(model.Language);
            return JsonOk();
        }

        public ActionResult ChangeLocale(string lang)
        {
            var cookie = new CookieHelper(HttpContext);
            cookie.InjectCookie(Mvc4WebRole.Helpers.UserLanguage.cookieName, new Language { Lang = lang });
            return RedirectToAction("Index");
        }


        #endregion
        #region passwordReset
        private const string SessionResetPassword = "SResetPassword";
        private const string ResetPasswordCrypticPropose = "reset password";
        [HttpGet]
        //issue with ie
        //[DonutOutputCache(VaryByParam = "none", VaryByCustom = CustomCacheKeys.Auth + ";"
        //   + CustomCacheKeys.Lang + ";"
        //   + CustomCacheKeys.Mobile, Duration = TimeConsts.Minute * 15)]
        public ActionResult ResetPassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword([ModelBinder(typeof(TrimModelBinder))]ForgotPassword model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                Guid membershipUserId;
                if (!m_MembershipService.Value.EmailExists(model.Email, out membershipUserId))
                {
                    ModelState.AddModelError("Email", AccountControllerResources.EmailDoesNotExists);
                    return View(model);
                }
                var query = new GetUserByMembershipQuery(membershipUserId);
                var result = await ZboxReadService.GetUserDetailsByMembershipId(query);
                var code = RandomString(10);


                var data = new ForgotPasswordLinkData(membershipUserId, 1);

                var linkData = CrypticElement(data);
                Session[SessionResetPassword] = data;
                m_QueueProvider.Value.InsertMessageToMailNew(new ForgotPasswordData2(code, linkData, result.Name.Split(' ')[0], model.Email, result.Culture));

                TempData["key"] = System.Web.Helpers.Crypto.HashPassword(code);

                return RedirectToAction("Confirmation");

            }

            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("ForgotPassword email: {0}", model.Email), ex);
                ModelState.AddModelError(string.Empty, AccountControllerResources.UnspecifiedError);
            }



            return View(model);
        }

        [HttpGet, NoCache]
        public ActionResult Confirmation()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
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
            return View(new Confirmation { Key = TempData["key"].ToString() });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [RequireHttps]
        public ActionResult Confirmation([ModelBinder(typeof(TrimModelBinder))] Confirmation model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
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
            return View(model);

        }

        [RequireHttps, HttpGet]
        public ActionResult PasswordUpdate(string key)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                return RedirectToAction("index");
            }
            var data = UnEncryptElement<ForgotPasswordLinkData>(key);
            if (data == null)
            {
                return RedirectToAction("index");
            }
            if (data.Date.AddHours(1) < DateTime.UtcNow)
            {
                return RedirectToAction("index");
            }
            return View(new NewPassword());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireHttps]
        public async Task<ActionResult> PasswordUpdate([ModelBinder(typeof(TrimModelBinder))] NewPassword model, string key)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                return View(model);
            }
            var data = UnEncryptElement<ForgotPasswordLinkData>(key);
            if (data == null)
            {
                return RedirectToAction("index");
            }
            if (data.Date.AddHours(1) < DateTime.UtcNow)
            {
                return RedirectToAction("index");
            }
            m_MembershipService.Value.ResetPassword(data.MembershipUserId, model.Password);

            var query = new GetUserByMembershipQuery(data.MembershipUserId);
            var result = await ZboxReadService.GetUserDetailsByMembershipId(query);
            Session.Abandon();
            FormsAuthenticationService.SignIn(result.Id, false,
                new UserDetail(
                    result.Culture,
                    result.UniversityId,
                    result.UniversityData
                    ));

            return RedirectToAction("Index", "Dashboard");

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



        [ChildActionOnly]
        public ActionResult GetUserDetail3()
        {
            const string userDetailView = "_UserDetail4";
            if (User == null || !(User.Identity.IsAuthenticated))
            {
                return PartialView(userDetailView);
            }
            try
            {
                var retVal = ZboxReadService.GetUserData(new GetUserDetailsQuery(User.GetUserId()));
                //  var userData = m_UserProfile.Value.GetUserData(ControllerContext);
                var serializer = new JsonNetSerializer();
                var jsonRetVal = serializer.Serialize(retVal);
                ViewBag.userDetail = jsonRetVal;
                return PartialView(userDetailView, retVal);
            }
            catch (UserNotFoundException)
            {
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("GetUserDetail user" + User.Identity.Name, ex);
                return new EmptyResult();
            }
        }

        [HttpGet]
        public ActionResult Details()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return JsonOk();
            }
            var retVal = ZboxReadService.GetUserData(new GetUserDetailsQuery(User.GetUserId()));
            return JsonOk(retVal);

        }


        [HttpPost, ZboxAuthorize]
        public JsonResult FirstTime(FirstTime firstTime)
        {
            var userid = User.GetUserId();
            var command = new UpdateUserFirstTimeStatusCommand(firstTime, userid);
            ZboxWriteService.UpdateUserFirstTimeStatus(command);

            return JsonOk();
        }

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

        [HttpGet]
        [OutputCache(CacheProfile = "PartialCache")]
        public ActionResult WelcomeAngularPartial()
        {
            try
            {
                return PartialView("_WelcomeAngular");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("_WelcomeAngular", ex);
                return Json(new JsonResponse(false));
            }
        }

    }


}
