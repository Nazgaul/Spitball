using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Microsoft.Owin.Security;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models.Account;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ViewModel.Dto.UserDtos;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class AccountController : BaseController
    {
        private readonly Lazy<IFacebookService> m_FacebookService;
        private readonly Lazy<IGoogleService> m_GoogleService;
        private readonly Lazy<IQueueProvider> m_QueueProvider;
        private readonly Lazy<IEncryptObject> m_EncryptObject;
        private readonly ApplicationUserManager m_UserManager;
        private readonly IAuthenticationManager m_AuthenticationManager;
        private readonly ICookieHelper m_CookieHelper;
        private readonly ILanguageCookieHelper m_LanguageCookie;
        //private readonly IThemeCookieHelper m_ThemeCookieHelper;


        public AccountController(
           Lazy<IFacebookService> facebookService,
           Lazy<IQueueProvider> queueProvider,
           Lazy<IEncryptObject> encryptObject,
           ApplicationUserManager userManager,
        IAuthenticationManager authenticationManager, ICookieHelper cookieHelper, ILanguageCookieHelper languageCookie, Lazy<IGoogleService> googleService)
        {
            m_FacebookService = facebookService;
            m_QueueProvider = queueProvider;
            m_EncryptObject = encryptObject;
            m_UserManager = userManager;
            m_AuthenticationManager = authenticationManager;
            m_CookieHelper = cookieHelper;
            m_LanguageCookie = languageCookie;
            m_GoogleService = googleService;
            //m_ThemeCookieHelper = themeCookieHelper;
        }


        public ActionResult Index(string lang, string invId)
        {
            return RedirectToActionPermanent("Index", "Home", new { lang, invId });
        }

        //[DonutOutputCache(VaryByParam = "lang;invId",
        //    VaryByCustom = CustomCacheKeys.Lang,
        //    Duration = TimeConsts.Second,
        //    Location = OutputCacheLocation.Server, Order = 2)]
        //public ActionResult SignIn()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Dashboard");
        //    }
        //    return View();
        //}
        //[DonutOutputCache(VaryByParam = "lang;invId",
        //   VaryByCustom = CustomCacheKeys.Lang,
        //   Duration = TimeConsts.Day,
        //   Location = OutputCacheLocation.Server, Order = 2)]
        //public ActionResult Signup(string invId)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Dashboard");
        //    }
        //    if (!string.IsNullOrEmpty(invId))
        //    {
        //        var guid = GuidEncoder.TryParseNullableGuid(invId);
        //        if (guid.HasValue)
        //        {
        //            m_CookieHelper.InjectCookie(Invite.CookieName, new Invite { InviteId = guid.Value });
        //        }
        //    }
        //    return View("Signin");
        //}






        #region Login

        [HttpPost]
        public async Task<JsonResult> GoogleLogin(ExternalLogIn model, string returnUrl, CancellationToken cancellationToken)
        {
            var source = CreateCancellationToken(cancellationToken);
            var googleUserData = await m_GoogleService.Value.GoogleLogInAsync(model.Token);
            if (googleUserData == null)
            {
                return JsonError(new { error = AccountControllerResources.FacebookGetDataError });
            }
            var query = new GetUserByGoogleQuery(googleUserData.Id);
            LogInUserDto user = await ZboxReadService.GetUserDetailsByGoogleIdAsync(query, source.Token);
            if (user == null)
            {

                var inv = m_CookieHelper.ReadCookie<Invite>(Invite.CookieName);
                Guid? invId = null;
                if (inv != null)
                {
                    invId = inv.InviteId;
                }
                model.BoxId = model.BoxId ?? GetBoxIdRouteDataFromDifferentUrl(returnUrl);
                var command = new CreateGoogleUserCommand(googleUserData.Email,
                    googleUserData.Id,
                    googleUserData.Picture,
                    model.UniversityId,
                    googleUserData.FirstName,
                    googleUserData.LastName,
                    googleUserData.Locale,
                    invId,
                    model.BoxId);
                var commandResult = await ZboxWriteService.CreateUserAsync(command);
                user = new LogInUserDto
                {
                    Id = commandResult.User.Id,
                    Culture = commandResult.User.Culture,
                    Image = googleUserData.Picture,
                    Name = googleUserData.Name,
                    UniversityId = commandResult.UniversityId,
                    UniversityData = commandResult.UniversityData,
                    Score = commandResult.User.Reputation
                };
            }
            m_LanguageCookie.InjectCookie(user.Culture);
            var identity = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimConsts.UserIdClaim, user.Id.ToString(CultureInfo.InvariantCulture)),
                   
                },
                "ApplicationCookie");
            if (user.UniversityId.HasValue)
            {
                identity.AddClaim(new Claim(ClaimConsts.UniversityIdClaim,
                    user.UniversityId.Value.ToString(CultureInfo.InvariantCulture)));
            }
            if (user.UniversityData.HasValue)
            {
                identity.AddClaim(new Claim(ClaimConsts.UniversityDataClaim,
                    user.UniversityData.Value.ToString(CultureInfo.InvariantCulture)));
            }


            m_AuthenticationManager.SignIn(identity);
            m_CookieHelper.RemoveCookie(Invite.CookieName);
            var url = user.UniversityId.HasValue
                   ? Url.Action("Index", "Dashboard")
                   : Url.Action("Choose", "Library");
            return JsonOk(url);
        }

        [HttpPost]
        [RequireHttps]
        public async Task<JsonResult> FacebookLogin(ExternalLogIn model, string returnUrl)
        {
            try
            {
                var facebookUserData = await m_FacebookService.Value.FacebookLogIn(model.Token);
                if (facebookUserData == null)
                {
                    return JsonError(new { error = AccountControllerResources.FacebookGetDataError });
                }
                var query = new GetUserByFacebookQuery(facebookUserData.Id);
                LogInUserDto user = await ZboxReadService.GetUserDetailsByFacebookId(query);
                if (user == null)
                {

                    var inv = m_CookieHelper.ReadCookie<Invite>(Invite.CookieName);
                    Guid? invId = null;
                    if (inv != null)
                    {
                        invId = inv.InviteId;
                    }
                    model.BoxId = model.BoxId ?? GetBoxIdRouteDataFromDifferentUrl(returnUrl);
                    var command = new CreateFacebookUserCommand(facebookUserData.Id, facebookUserData.Email,
                         facebookUserData.LargeImage, model.UniversityId,
                        facebookUserData.First_name,

                        facebookUserData.Last_name,
                        facebookUserData.Locale, invId, model.BoxId);
                    var commandResult = await ZboxWriteService.CreateUserAsync(command);
                    user = new LogInUserDto
                    {
                        Id = commandResult.User.Id,
                        Culture = commandResult.User.Culture,
                        Image = facebookUserData.LargeImage,
                        Name = facebookUserData.Name,
                        UniversityId = commandResult.UniversityId,
                        UniversityData = commandResult.UniversityData,
                        Score = commandResult.User.Reputation
                    };
                }
                m_LanguageCookie.InjectCookie(user.Culture);
                var identity = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimConsts.UserIdClaim, user.Id.ToString(CultureInfo.InvariantCulture)),
                   
                },
                    "ApplicationCookie");
                if (user.UniversityId.HasValue)
                {
                    identity.AddClaim(new Claim(ClaimConsts.UniversityIdClaim,
                        user.UniversityId.Value.ToString(CultureInfo.InvariantCulture)));
                }
                if (user.UniversityData.HasValue)
                {
                    identity.AddClaim(new Claim(ClaimConsts.UniversityDataClaim,
                        user.UniversityData.Value.ToString(CultureInfo.InvariantCulture)));
                }


                m_AuthenticationManager.SignIn(identity);
                m_CookieHelper.RemoveCookie(Invite.CookieName);
                var url = user.UniversityId.HasValue
                      ? Url.Action("Index", "Dashboard")
                      : Url.Action("Choose", "Library");
                return JsonOk(url);
            }
            catch (ArgumentException)
            {
                return JsonError(new { error = AccountControllerResources.FacebookGetDataError });
            }
            catch (NullReferenceException ex)
            {
                TraceLog.WriteError("token: " + model.Token, ex);
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
        public async Task<JsonResult> LogIn(
            [ModelBinder(typeof(TrimModelBinder))]LogOn model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            try
            {
                var query = new GetUserByEmailQuery(model.Email);
                var tSystemData = ZboxReadService.GetUserDetailsByEmail(query);
                var tUserIdentity = m_UserManager.FindByEmailAsync(model.Email);

                await Task.WhenAll(tSystemData, tUserIdentity);

                var user = tUserIdentity.Result;
                var systemUser = tSystemData.Result;
                if (systemUser == null)
                {
                    ModelState.AddModelError(string.Empty, AccountControllerResources.LogonError);
                    return JsonError(GetErrorFromModelState());
                }
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, AccountValidation.ErrorCodeToString(AccountValidation.AccountErrors.InvalidEmail));
                    return JsonError(GetErrorFromModelState());
                }


                var loginStatus = await m_UserManager.CheckPasswordAsync(user, model.Password);

                if (loginStatus)
                {
                    var identity = await user.GenerateUserIdentityAsync(m_UserManager, systemUser.Id,
                        systemUser.UniversityId, systemUser.UniversityData);
                    m_AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                    }, identity);

                    m_CookieHelper.RemoveCookie(Invite.CookieName);
                    m_LanguageCookie.InjectCookie(systemUser.Culture);

                    var url = systemUser.UniversityId.HasValue
                        ? Url.Action("Index", "Dashboard")
                        : Url.Action("Choose", "Library");
                    return JsonOk(url);

                }
                ModelState.AddModelError(string.Empty, AccountValidation.ErrorCodeToString(AccountValidation.AccountErrors.InvalidPassword));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("LogOn model : {0} ", model), ex);
                ModelState.AddModelError(string.Empty, AccountControllerResources.LogonError);
            }
            return JsonError(GetErrorFromModelState());
        }


        //private string CheckIfToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return returnUrl;
        //    }
        //    return string.Empty;
        //}



        //TODO: do a post on log out
        [RemoveBoxCookie]
        public ActionResult LogOff()
        {
            //if (Session != null)
            //Session.Abandon(); // remove the session cookie from user computer. wont continue session if user log in with a diffrent id.            
            m_AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Register([ModelBinder(typeof(TrimModelBinder))] Register model)
        {
            model.BoxId = GetBoxIdRouteDataFromDifferentUrl(model.ReturnUrl);
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            try
            {
                var inv = m_CookieHelper.ReadCookie<Invite>(Invite.CookieName);
                // Guid userProviderKey;

                var user = new ApplicationUser
                {
                    UserName = model.NewEmail,
                    Email = model.NewEmail
                };
                var createStatus = await m_UserManager.CreateAsync(user, model.Password);
                if (createStatus.Succeeded)
                {
                    Guid? invId = null;
                    if (inv != null)
                    {
                        invId = inv.InviteId;
                    }
                    var lang = m_LanguageCookie.ReadCookie();
                    if (!Languages.CheckIfLanguageIsSupported(lang))
                    {
                        lang = Thread.CurrentThread.CurrentCulture.Name;
                    }
                    CreateUserCommand command = new CreateMembershipUserCommand(Guid.Parse(user.Id),
                        model.NewEmail, model.UniversityId,
                        model.FirstName, model.LastName,
                        lang, invId, model.BoxId);
                    var result = await ZboxWriteService.CreateUserAsync(command);
                    m_LanguageCookie.InjectCookie(result.User.Culture);

                    var identity = await user.GenerateUserIdentityAsync(m_UserManager, result.User.Id, result.UniversityId,
                         result.UniversityData);

                    m_AuthenticationManager.SignIn(identity);

                    var url = result.UniversityId.HasValue ? Url.Action("Index", "Dashboard") : Url.Action("Choose", "Library");
                    m_CookieHelper.RemoveCookie(Invite.CookieName);
                    return JsonOk(url);


                }
                foreach (var error in createStatus.Errors)
                {
                    ModelState.AddModelError(string.Empty, AccountValidation.Localize(error, model.NewEmail));
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, BaseControllerResources.UnspecifiedError);
                TraceLog.WriteError("Register model:" + model, ex);
            }
            return JsonError(GetErrorFromModelState());
        }
        #endregion


        [ZboxAuthorize, NoUniversity]
        public async Task<JsonResult> SettingsData()
        {
            var userId = User.GetUserId();
            var query = new GetUserDetailsQuery(userId);

            var user = await ZboxReadService.GetUserAccountDetailsAsync(query);
            return JsonOk(user);
        }

        [DonutOutputCache(CacheProfile = "PartialPage")]
        [ZboxAuthorize, NoUniversity]
        public PartialViewResult SettingPartial()
        {
            return PartialView("Settings2");
        }

        const string SessionKey = "UserVerificationCode";
        [HttpPost]
        [ZboxAuthorize]
        public async Task<JsonResult> EnterCode(long? code)
        {
            if (!code.HasValue)
            {
                return JsonError(AccountControllerResources.ChangeEmailCodeError);
            }
            var model = m_CookieHelper.ReadCookie<ChangeMail>(SessionKey);
            if (model == null)
            {
                return JsonError(AccountControllerResources.ChangeEmailCodeError);
            }
            if (model.TimeOfExpire < DateTime.UtcNow)
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
                await ZboxWriteService.UpdateUserEmailAsync(command);
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
            m_CookieHelper.RemoveCookie(SessionKey);
            return JsonOk(model.Email);
        }

        [HttpPost]
        [ZboxAuthorize]
        public async Task<JsonResult> ChangeEmail(ChangeMail model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            try
            {
                var rand = new Random();
                var generatedCode = rand.Next(10000, 99999);
                model.Code = generatedCode;
                model.TimeOfExpire = DateTime.UtcNow.AddHours(3);
                m_CookieHelper.InjectCookie(SessionKey, model);

                TraceLog.WriteInfo("Sending change email with code" + model);
                await m_QueueProvider.Value.InsertMessageToMailNewAsync(new ChangeEmailData(generatedCode.ToString(CultureInfo.InvariantCulture),
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
            if (!ModelState.IsValid)
            {
                return JsonError(new { error = GetErrorFromModelState() });
            }

            var command = new UpdateUserProfileCommand(User.GetUserId(),
                model.FirstName,
                model.LastName);
            ZboxWriteService.UpdateUserProfile(command);
            return JsonOk();

        }
        [HttpPost, ZboxAuthorize]
        public async Task<ActionResult> UpdateUniversity(University model)
        {
            var needId = await ZboxReadService.GetUniversityNeedIdAsync(model.UniversityId);
            if (needId != null && string.IsNullOrEmpty(model.StudentId))
            {
                return JsonOk(needId);
            }

            if (!ModelState.IsValid)
            {
                return JsonError(new { error = GetErrorFromModelState() });
            }

            try
            {
                var id = User.GetUserId();
                var command = new UpdateUserUniversityCommand(model.UniversityId, id, model.StudentId);
                ZboxWriteService.UpdateUserUniversity(command);

                var user = (ClaimsIdentity)User.Identity;
                var claimUniversity = user.Claims.SingleOrDefault(w => w.Type == ClaimConsts.UniversityIdClaim);
                var claimUniversityData = user.Claims.SingleOrDefault(w => w.Type == ClaimConsts.UniversityDataClaim);

                if (claimUniversity != null)
                {
                    user.RemoveClaim(claimUniversity);
                }
                if (claimUniversityData != null)
                {
                    user.RemoveClaim(claimUniversityData);
                }


                user.AddClaim(new Claim(ClaimConsts.UniversityIdClaim,
                        command.UniversityId.ToString(CultureInfo.InvariantCulture)));

                user.AddClaim(new Claim(ClaimConsts.UniversityDataClaim,
                        command.UniversityDataId.HasValue ?
                        command.UniversityDataId.Value.ToString(CultureInfo.InvariantCulture)
                        : command.UniversityId.ToString(CultureInfo.InvariantCulture)));


                m_AuthenticationManager.SignIn(user);



                return JsonOk();
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError("Code", Models.Account.Resources.AccountSettingsResources.CodeIncorrect);
                return JsonError(GetErrorFromModelState());

            }
            catch (NullReferenceException)
            {
                ModelState.AddModelError("Code", Models.Account.Resources.AccountSettingsResources.CodeIncorrect);
                return JsonError(GetErrorFromModelState());

            }
            catch (Exception ex)
            {
                TraceLog.WriteError("update university model " + model, ex);
                ModelState.AddModelError("Code", Models.Account.Resources.AccountSettingsResources.CodeIncorrect);
                return JsonError(GetErrorFromModelState());
            }
        }


        [HttpPost]
        [ZboxAuthorize]
        public async Task<JsonResult> ChangePassword(Password model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var id = User.GetUserId();
            var command = new UpdateUserPasswordCommand(id, model.CurrentPassword, model.NewPassword);
            var commandResult = await ZboxWriteService.UpdateUserPasswordAsync(command);
            return Json(new JsonResponse(!commandResult.HasErrors, commandResult.Error));
        }


        [HttpPost]
        public JsonResult ChangeLocale(string language)
        {
            if (User.Identity.IsAuthenticated)
            {
                var id = User.GetUserId();
                var command = new UpdateUserLanguageCommand(id, language);
                ZboxWriteService.UpdateUserLanguage(command);
            }
            m_LanguageCookie.InjectCookie(language);
            return JsonOk();
        }

        [HttpPost]
        public JsonResult ChangeTheme(Theme theme)
        {
            if (User.Identity.IsAuthenticated)
            {
                var id = User.GetUserId();
                var command = new UpdateUserThemeCommand(id, theme);
                ZboxWriteService.UpdateUserTheme(command);
            }
            //m_ThemeCookieHelper.InjectCookie(theme);
            return JsonOk();
        }



        #region passwordReset
        //[HttpGet]
        //issue with ie
        //[DonutOutputCache(VaryByParam = "none", VaryByCustom = CustomCacheKeys.Auth + ";"
        //   + CustomCacheKeys.Lang + ";"
        //   + CustomCacheKeys.Mobile, Duration = TimeConsts.Minute * 15)]
        //public ActionResult ResetPassword()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Dashboard");
        //    }
        //    return View("Signin");
        //}

        [HttpPost]
        public async Task<JsonResult> ResetPassword([ModelBinder(typeof(TrimModelBinder))]ForgotPassword model, CancellationToken cancellationToken)
        {

            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            try
            {
                var source = CreateCancellationToken(cancellationToken);
                var query = new GetUserByEmailQuery(model.Email);
                var tUser = m_UserManager.FindByEmailAsync(model.Email);
                var tResult = ZboxReadService.GetForgotPasswordByEmailAsync(query, source.Token);

                await Task.WhenAll(tUser, tResult);

                var systemUser = tResult.Result;
                if (systemUser == null)
                {
                    TraceLog.WriteInfo("Email not exists " + model);
                    return JsonError(AccountControllerResources.EmailDoesNotExists);
                }

                if (systemUser.IdentityId.HasValue)
                {
                    var user = tUser.Result;
                    var identitylinkData = await m_UserManager.GeneratePasswordResetTokenAsync(user.Id);

                    var data = new ForgotPasswordLinkData(Guid.Parse(user.Id), 1, identitylinkData);

                    var linkData = EncryptElement(data);
                    await m_QueueProvider.Value.InsertMessageToMailNewAsync(new ForgotPasswordData2(linkData, tResult.Result.FirstName, model.Email, tResult.Result.Culture));

                    return JsonOk();
                }
                if (systemUser.FacebookId.HasValue)
                {
                    TraceLog.WriteInfo("facebook user " + model);
                    return JsonError(AccountControllerResources.FbRegisterError);
                }
                if (!string.IsNullOrEmpty(systemUser.GoogleId))
                {
                    TraceLog.WriteInfo("google user " + model);
                    return JsonError(AccountControllerResources.GoogleForgotPasswordError);
                }
                TraceLog.WriteInfo("Email not exists " + model);
                return JsonError(AccountControllerResources.EmailDoesNotExists);

            }

            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("ForgotPassword email: {0}", model.Email), ex);
                return JsonError(BaseControllerResources.UnspecifiedError);
            }

        }

        [HttpGet]
        public ActionResult PasswordUpdate(string key)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                return RedirectToRoute("Default");
            }
            var data = UnEncryptElement(key);
            if (data == null)
            {
                return RedirectToRoute("Default");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
            var data = UnEncryptElement(key);
            if (data == null)
            {
                return RedirectToAction("index");
            }
            var result = await m_UserManager.ResetPasswordAsync(data.MembershipUserId.ToString(), data.Hash, model.Password);

            if (result.Succeeded)
            {
                var query = new GetUserByMembershipQuery(data.MembershipUserId);
                var tSystemUser = ZboxReadService.GetUserDetailsByMembershipId(query);

                var tUser = m_UserManager.FindByIdAsync(data.MembershipUserId.ToString());

                await Task.WhenAll(tSystemUser, tUser);

                var identity = await tUser.Result.GenerateUserIdentityAsync(m_UserManager, tSystemUser.Result.Id,
                       tSystemUser.Result.UniversityId, tSystemUser.Result.UniversityData);
                m_AuthenticationManager.SignIn(identity);
                return RedirectToAction("Index", "Dashboard");
            }
            ModelState.AddModelError(string.Empty, AccountControllerResources.AccountController_PasswordUpdate_Error);
            return View(model);
        }


        //[NonAction]
        //private string RandomString(int size)
        //{
        //    var random = new Random();
        //    const string input = "0123456789";
        //    var chars = Enumerable.Range(0, size)
        //                           .Select(x => input[random.Next(0, input.Length)]);
        //    return new string(chars.ToArray());
        //    //return "12345";
        //}

        private const string ResetPasswordCrypticPropose = "reset password";
        [NonAction]
        private string EncryptElement(ForgotPasswordLinkData obj)
        {
            return m_EncryptObject.Value.EncryptElement(obj, ResetPasswordCrypticPropose);

        }
        [NonAction]
        private ForgotPasswordLinkData UnEncryptElement(string str)
        {
            return m_EncryptObject.Value.DecryptElement<ForgotPasswordLinkData>(str, ResetPasswordCrypticPropose);

        }
        #endregion

        //[ChildActionOnly]
        //public ActionResult GetUserDetail3()
        //{
        //    const string userDetailView = "_UserDetailStatic";
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
        public async Task<JsonResult> Details()
        {
            //string cookieToken, formToken;
            //AntiForgery.GetTokens(null, out cookieToken, out formToken);
            //var token = formToken;

            //var cookieHelper = new CookieHelper(HttpContext);
            //cookieHelper.InjectCookie(AntiForgeryConfig.CookieName, cookieToken);

            if (!User.Identity.IsAuthenticated)
            {
                return JsonOk(new { /*token,*/ Culture = CultureInfo.CurrentCulture.Name });
            }
            var retVal = await ZboxReadService.GetUserDataAsync(new GetUserDetailsQuery(User.GetUserId()));
            //retVal.Token = token;
            return JsonOk(retVal);

        }

        [HttpGet, ZboxAuthorize]
        public PartialViewResult Info()
        {
            return PartialView();
        }
        [HttpGet, ZboxAuthorize]
        public PartialViewResult Password()
        {
            return PartialView();
        }
        [HttpGet, ZboxAuthorize]
        public PartialViewResult Notification()
        {
            return PartialView();
        }

        [HttpGet, ZboxAuthorize]
        public PartialViewResult Department()
        {
            return PartialView();
        }


        //protected override void Dispose(bool disposing)
        //{
        //    m_UserManager.Dispose();
        //    base.Dispose(disposing);
        //}
    }


}
