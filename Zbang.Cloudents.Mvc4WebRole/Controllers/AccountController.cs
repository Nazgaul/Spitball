using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models.Account;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Extensions;
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
        private readonly Lazy<ApplicationUserManager> m_UserManager;
        private readonly IAuthenticationManager m_AuthenticationManager;
        private readonly ICookieHelper m_CookieHelper;
        private readonly ILanguageCookieHelper m_LanguageCookie;

        public AccountController(
           Lazy<IFacebookService> facebookService,
           Lazy<IQueueProvider> queueProvider,
           Lazy<IEncryptObject> encryptObject,
           Lazy<ApplicationUserManager> userManager,
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
        }


        public ActionResult Index(string lang, string invId)
        {
            return RedirectToRoutePermanent("homePage", new { lang, invId });
        }



        #region Login

        [HttpPost, ActionName("GoogleLogin")]
        public async Task<JsonResult> GoogleLoginAsync(ExternalLogIn model, CancellationToken cancellationToken)
        {

            var googleUserData = await m_GoogleService.Value.GoogleLogOnAsync(model.Token);
            if (googleUserData == null)
            {
                return JsonError(new { error = AccountControllerResources.FacebookGetDataError });
            }
            var query = new GetUserByGoogleQuery(googleUserData.Id);
            using (var source = CreateCancellationToken(cancellationToken))
            {
                var user = await ZboxReadService.GetUserDetailsByGoogleIdAsync(query, source.Token);
                if (user == null)
                {

                    var inv = m_CookieHelper.ReadCookie<Invite>(Invite.CookieName);
                    var university = m_CookieHelper.ReadCookie<UniversityCookie>(UniversityCookie.CookieName);


                    model.BoxId = model.BoxId ?? GetBoxIdRouteDataFromDifferentUrl();
                    var command = new CreateGoogleUserCommand(googleUserData.Email,
                        googleUserData.Id,
                        googleUserData.Picture,
                        //ExtractUniversityId(),
                        googleUserData.FirstName,
                        googleUserData.LastName,
                        googleUserData.Locale, Sex.NotKnown,
                        ExtractQueryStringFromUrlReferrer(),
                        inv?.InviteId,
                        model.BoxId,
                        university?.UniversityId
                        );
                    try
                    {
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
                    catch (UserRegisterFacebookException)
                    {
                        return JsonError(new { error = AccountControllerResources.RegisterEmailFacebookAccountError });
                    }
                    catch (UserRegisterGoogleException)
                    {
                        return JsonError(new { error = AccountControllerResources.RegisterEmailGoogleAccountError });
                    }
                    catch (UserRegisterEmailException)
                    {
                        return JsonError(new { error = AccountControllerResources.RegisterEmailDuplicate });
                    }
                }
                m_LanguageCookie.InjectCookie(user.Culture);
                var identity = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimConst.UserIdClaim, user.Id.ToString(CultureInfo.InvariantCulture)),

                },
                    "ApplicationCookie");
                if (user.UniversityId.HasValue)
                {
                    identity.AddClaim(new Claim(ClaimConst.UniversityIdClaim,
                        user.UniversityId.Value.ToString(CultureInfo.InvariantCulture)));
                }
                if (user.UniversityData.HasValue)
                {
                    identity.AddClaim(new Claim(ClaimConst.UniversityDataClaim,
                        user.UniversityData.Value.ToString(CultureInfo.InvariantCulture)));
                }


                m_AuthenticationManager.SignIn(new AuthenticationProperties
                {
                    IsPersistent = true,
                }, identity);
                m_CookieHelper.RemoveCookie(Invite.CookieName);
                m_CookieHelper.RemoveCookie(UniversityCookie.CookieName);
                var url = user.UniversityId.HasValue
                    ? Url.Action("Index", "Dashboard")
                    : Url.Action("Choose", "University");
                return JsonOk(url);
            }
        }


        private NameValueCollection ExtractQueryStringFromUrlReferrer()
        {
            var query = HttpContext.Request.UrlReferrer?.Query;
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }
            return HttpUtility.ParseQueryString(query);
        }

        private long? GetBoxIdRouteDataFromDifferentUrl()
        {
            try
            {
                var queryParsed = ExtractQueryStringFromUrlReferrer();
                var url = queryParsed?["returnUrl"];

                if (string.IsNullOrEmpty(url))
                {
                    return null;
                }
                var routeData = BuildRouteDataFromUrl(url);
                //if (BuildRouteDataFromUrl(url, out routeData)) return null;
                if (routeData?.Values["boxId"] == null)
                {
                    return null;
                }
                long retVal;
                if (long.TryParse(routeData.Values["boxId"].ToString(), out retVal))
                {
                    return retVal;
                }
                return null;

            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"GetBoxIdRouteDataFromDifferentUrl url: {HttpContext.Request.UrlReferrer}", ex);
                return null;
            }

        }




        [HttpPost]
        [ActionName("FacebookLogin")]
        public async Task<JsonResult> FacebookLoginAsync(ExternalLogIn model)
        {
            try
            {
                var facebookUserData = await m_FacebookService.Value.FacebookLogOnAsync(model.Token);
                if (facebookUserData == null)
                {
                    return JsonError(new { error = AccountControllerResources.FacebookGetDataError });
                }
                var query = new GetUserByFacebookQuery(facebookUserData.Id);
                var user = await ZboxReadService.GetUserDetailsByFacebookId(query);
                if (user == null)
                {

                    var inv = m_CookieHelper.ReadCookie<Invite>(Invite.CookieName);
                    var university = m_CookieHelper.ReadCookie<UniversityCookie>(UniversityCookie.CookieName);

                    model.BoxId = model.BoxId ?? GetBoxIdRouteDataFromDifferentUrl();

                    var command = new CreateFacebookUserCommand(facebookUserData.Id, facebookUserData.Email,
                        facebookUserData.LargeImage,
                        facebookUserData.First_name,
                        facebookUserData.Last_name,
                        facebookUserData.Locale, facebookUserData.SpitballGender, ExtractQueryStringFromUrlReferrer(),
                        inv?.InviteId,
                        model.BoxId,
                        university?.UniversityId);
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
                    new Claim(ClaimConst.UserIdClaim, user.Id.ToString(CultureInfo.InvariantCulture)),

                },
                    "ApplicationCookie");
                if (user.UniversityId.HasValue)
                {
                    identity.AddClaim(new Claim(ClaimConst.UniversityIdClaim,
                        user.UniversityId.Value.ToString(CultureInfo.InvariantCulture)));
                }
                if (user.UniversityData.HasValue)
                {
                    identity.AddClaim(new Claim(ClaimConst.UniversityDataClaim,
                        user.UniversityData.Value.ToString(CultureInfo.InvariantCulture)));
                }


                m_AuthenticationManager.SignIn(new AuthenticationProperties
                {
                    IsPersistent = true,
                }, identity);
                m_CookieHelper.RemoveCookie(Invite.CookieName);
                m_CookieHelper.RemoveCookie(UniversityCookie.CookieName);
                var url = user.UniversityId.HasValue
                      ? Url.Action("Index", "Dashboard")
                      : Url.Action("Choose", "University");
                return JsonOk(url);
            }
            catch (UserRegisterFacebookException)
            {
                return JsonError(new { error = AccountControllerResources.RegisterEmailFacebookAccountError });
            }
            catch (UserRegisterGoogleException)
            {
                return JsonError(new { error = AccountControllerResources.RegisterEmailGoogleAccountError });
            }
            catch (UserRegisterEmailException)
            {
                return JsonError(new { error = AccountControllerResources.RegisterEmailDuplicate });
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
        [/*ValidateAntiForgeryToken,*/ActionName("LogIn")]
        public async Task<JsonResult> LogInAsync(
            /*[ModelBinder(typeof(TrimModelBinder))]*/LogOn model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            try
            {
                var query = new GetUserByEmailQuery(model.Email);
                var tSystemData = ZboxReadService.GetUserDetailsByEmail(query);
                var tUserIdentity = m_UserManager.Value.FindByEmailAsync(model.Email);

                await Task.WhenAll(tSystemData, tUserIdentity);

                var user = tUserIdentity.Result;
                var systemUser = tSystemData.Result;


                if (systemUser == null)
                {
                    ModelState.AddModelError(string.Empty, AccountControllerResources.LogonError);
                    return JsonError(GetErrorFromModelState());
                }

                if (systemUser.MembershipId.HasValue)
                {
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, AccountValidation.ErrorCodeToString(AccountValidation.AccountError.InvalidEmail));
                        return JsonError(GetErrorFromModelState());
                    }
                    var loginStatus = await m_UserManager.Value.CheckPasswordAsync(user, model.Password);

                    if (loginStatus)
                    {
                        var identity = await user.GenerateUserIdentityAsync(m_UserManager.Value, systemUser.Id,
                            systemUser.UniversityId, systemUser.UniversityData);
                        m_AuthenticationManager.SignIn(new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                        }, identity);

                        m_CookieHelper.RemoveCookie(Invite.CookieName);
                        m_LanguageCookie.InjectCookie(systemUser.Culture);

                        var url = systemUser.UniversityId.HasValue
                            ? Url.Action("Index", "Dashboard")
                            : Url.Action("Choose", "University");
                        return JsonOk(url);

                    }
                    ModelState.AddModelError(string.Empty, AccountValidation.ErrorCodeToString(AccountValidation.AccountError.InvalidPassword));
                    return JsonError(GetErrorFromModelState());
                }
                if (systemUser.FacebookId.HasValue)
                {
                    ModelState.AddModelError(string.Empty, AccountControllerResources.RegisterEmailFacebookAccountError);
                    return JsonError(GetErrorFromModelState());
                }
                if (!string.IsNullOrEmpty(systemUser.GoogleId))
                {
                    ModelState.AddModelError(string.Empty, AccountControllerResources.RegisterEmailGoogleAccountError);
                    return JsonError(GetErrorFromModelState());
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"LogOn model : {model} ", ex);
                ModelState.AddModelError(string.Empty, AccountControllerResources.LogonError);
            }
            return JsonError(GetErrorFromModelState());
        }

        [RemoveBoxCookie]
        public ActionResult LogOff()
        {
            //if (Session != null)
            //Session.Abandon(); // remove the session cookie from user computer. wont continue session if user log in with a diffrent id.            
            m_CookieHelper.RemoveCookie(UploadController.ChatCookieName);
            m_CookieHelper.RemoveCookie(UploadController.UploadcookieName);
            m_CookieHelper.RemoveCookie(BoxPermissionAttribute.Permission.CookieName);
            m_AuthenticationManager.SignOut();
            return RedirectToRoute("homePage");
        }





        [HttpPost]
        [/*ValidateAntiForgeryToken,*/ActionName("Register")]
        public async Task<JsonResult> RegisterAsync(/*[ModelBinder(typeof(TrimModelBinder))]*/ Register model)
        {
            model.BoxId = GetBoxIdRouteDataFromDifferentUrl();
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            IdentityResult createStatus = null;
            try
            {
                var inv = m_CookieHelper.ReadCookie<Invite>(Invite.CookieName);
                var universityId = m_CookieHelper.ReadCookie<UniversityCookie>(UniversityCookie.CookieName);
                // Guid userProviderKey;

                var user = new ApplicationUser
                {
                    UserName = model.NewEmail,
                    Email = model.NewEmail
                };
                createStatus = await m_UserManager.Value.CreateAsync(user, model.Password);
                if (createStatus.Succeeded)
                {
                    var lang = m_LanguageCookie.ReadCookie();
                    if (!Languages.CheckIfLanguageIsSupported(lang))
                    {
                        lang = Thread.CurrentThread.CurrentCulture.Name;
                    }
                    CreateUserCommand command = new CreateMembershipUserCommand(Guid.Parse(user.Id),
                        model.NewEmail,
                        model.FirstName, model.LastName,
                        lang, model.Sex, ExtractQueryStringFromUrlReferrer(), inv?.InviteId, model.BoxId, universityId?.UniversityId);
                    var result = await ZboxWriteService.CreateUserAsync(command);
                    m_LanguageCookie.InjectCookie(result.User.Culture);

                    var identity =
                        await user.GenerateUserIdentityAsync(m_UserManager.Value, result.User.Id, result.UniversityId,
                            result.UniversityData);

                    m_AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true,
                    }, identity);

                    var url = result.UniversityId.HasValue
                        ? Url.Action("Index", "Dashboard")
                        : Url.Action("Choose", "University");
                    m_CookieHelper.RemoveCookie(Invite.CookieName);
                    m_CookieHelper.RemoveCookie(UniversityCookie.CookieName);
                    return JsonOk(url);


                }
                foreach (var error in createStatus.Errors)
                {
                    ModelState.AddModelError(string.Empty, AccountValidation.Localize(error, model.NewEmail));
                }
            }
            catch (UserRegisterFacebookException)
            {
                ModelState.AddModelError(string.Empty, AccountControllerResources.RegisterEmailFacebookAccountError);
            }
            catch (UserRegisterGoogleException)
            {
                ModelState.AddModelError(string.Empty, AccountControllerResources.RegisterEmailGoogleAccountError);
            }
            catch (UserRegisterEmailException)
            {
                ModelState.AddModelError(string.Empty, AccountControllerResources.RegisterEmailDuplicate);
            }
            catch (DbEntityValidationException ex)
            {
                ModelState.AddModelError(string.Empty, BaseControllerResources.UnspecifiedError);
                foreach (var dbEntityValidationResult in ex.EntityValidationErrors)
                {
                    TraceLog.WriteError(string.Join(" ",
                        dbEntityValidationResult.ValidationErrors.Select(s => s.ErrorMessage)));
                }
                TraceLog.WriteError("Register model:" + model, ex);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, BaseControllerResources.UnspecifiedError);
                TraceLog.WriteError("Register model:" + model, ex);
            }
            if (createStatus != null && createStatus.Succeeded)
            {
                var user = await m_UserManager.Value.FindByEmailAsync(model.NewEmail);

                await m_UserManager.Value.DeleteAsync(user);
            }
            return JsonError(GetErrorFromModelState());
        }
        #endregion


        [ZboxAuthorize, NoUniversity, ActionName("SettingsData")]
        public async Task<JsonResult> SettingsDataAsync()
        {
            var userId = User.GetUserId();
            var query = new QueryBaseUserId(userId);

            var user = await ZboxReadService.GetUserAccountDetailsAsync(query);
            return JsonOk(user);
        }


        [DonutOutputCache(CacheProfile = "PartialPage")]
        [ZboxAuthorize]
        public ActionResult UserDetails()
        {
            return PartialView();
        }

        [DonutOutputCache(CacheProfile = "PartialPage")]

        public ActionResult UnregisterView()
        {
            return PartialView();
        }

        [DonutOutputCache(CacheProfile = "PartialPage")]
        [ZboxAuthorize, NoUniversity]
        public PartialViewResult SettingPartial()
        {
            return PartialView("Settings2");
        }

        const string SessionKey = "UserVerificationCode";
        [HttpPost]
        [ZboxAuthorize, ActionName("EnterCode")]
        public async Task<JsonResult> EnterCodeAsync(long? code)
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
        [ZboxAuthorize, ActionName("ChangeEmail")]
        public async Task<JsonResult> ChangeEmailAsync(ChangeMail model)
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
        public ActionResult ChangeProfile(/*[ModelBinder(typeof(TrimModelBinder))]*/Profile model)
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
        [HttpPost, ZboxAuthorize, ActionName("UpdateUniversity")]
        public async Task<ActionResult> UpdateUniversityAsync(University model)
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
                var claimUniversity = user.Claims.SingleOrDefault(w => w.Type == ClaimConst.UniversityIdClaim);
                var claimUniversityData = user.Claims.SingleOrDefault(w => w.Type == ClaimConst.UniversityDataClaim);

                if (claimUniversity != null)
                {
                    user.RemoveClaim(claimUniversity);
                }
                if (claimUniversityData != null)
                {
                    user.RemoveClaim(claimUniversityData);
                }


                user.AddClaim(new Claim(ClaimConst.UniversityIdClaim,
                        command.UniversityId.ToString(CultureInfo.InvariantCulture)));

                user.AddClaim(new Claim(ClaimConst.UniversityDataClaim,
                        command.UniversityDataId?.ToString(CultureInfo.InvariantCulture) ?? command.UniversityId.ToString(CultureInfo.InvariantCulture)));

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
        [ZboxAuthorize, ActionName("ChangePassword")]
        public async Task<JsonResult> ChangePasswordAsync(Password model)
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

        //[HttpPost]
        //public JsonResult ChangeTheme(Theme theme)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var id = User.GetUserId();
        //        var command = new UpdateUserThemeCommand(id, theme);
        //        ZboxWriteService.UpdateUserTheme(command);
        //    }
        //    //m_ThemeCookieHelper.InjectCookie(theme);
        //    return JsonOk();
        //}



        #region passwordReset


        [HttpPost, ActionName("ResetPassword")]
        public async Task<JsonResult> ResetPasswordAsync(/*[ModelBinder(typeof(TrimModelBinder))]*/ForgotPassword model, CancellationToken cancellationToken)
        {

            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            try
            {
                using (var source = CreateCancellationToken(cancellationToken))
                {
                    var query = new GetUserByEmailQuery(model.Email);
                    var tUser = m_UserManager.Value.FindByEmailAsync(model.Email);
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
                        var identitylinkData = await m_UserManager.Value.GeneratePasswordResetTokenAsync(user.Id);

                        var data = new ForgotPasswordLinkData(Guid.Parse(user.Id), 1, identitylinkData);

                        var linkData = EncryptElement(data);
                        await
                            m_QueueProvider.Value.InsertMessageToMailNewAsync(new ForgotPasswordData2(linkData,
                                tResult.Result.FirstName, model.Email, tResult.Result.Culture));

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
            }

            catch (Exception ex)
            {
                TraceLog.WriteError($"ForgotPassword email: {model.Email}", ex);
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
                return RedirectToRoute("HomePage");
            }
            var data = UnEncryptElement(key);
            if (data == null)
            {
                return RedirectToRoute("HomePage");
            }
            return View();
        }

        [HttpPost]
        [/*ValidateAntiForgeryToken,*/ActionName("PasswordUpdate")]
        public async Task<ActionResult> PasswordUpdateAsync(/*[ModelBinder(typeof(TrimModelBinder))]*/ NewPassword model, string key)
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
            var result = await m_UserManager.Value.ResetPasswordAsync(data.MembershipUserId.ToString(), data.Hash, model.Password);

            if (result.Succeeded)
            {
                var query = new GetUserByMembershipQuery(data.MembershipUserId);
                var tSystemUser = ZboxReadService.GetUserDetailsByMembershipId(query);

                var tUser = m_UserManager.Value.FindByIdAsync(data.MembershipUserId.ToString());

                await Task.WhenAll(tSystemUser, tUser);

                var identity = await tUser.Result.GenerateUserIdentityAsync(m_UserManager.Value, tSystemUser.Result.Id,
                       tSystemUser.Result.UniversityId, tSystemUser.Result.UniversityData);
                m_AuthenticationManager.SignIn(identity);
                return RedirectToAction("Index", "Dashboard");
            }
            ModelState.AddModelError(string.Empty, AccountControllerResources.AccountController_PasswordUpdate_Error);
            return View(model);
        }




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



        [HttpGet, ActionName("Details")]
        public async Task<ActionResult> DetailsAsync()
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
            try
            {
                //TODO : merge that
                var command = new AddUserLocationActivityCommand(User.GetUserId(), HttpContext.Request.UserAgent);
                await ZboxWriteService.AddUserLocationActivityAsync(command);
                var retVal = await ZboxReadService.GetUserDataAsync(new QueryBaseUserId(User.GetUserId()));

                var level = GamificationLevels.GetLevel(retVal.Score);
                retVal.LevelName = level.Name;
                retVal.NextLevel = level.NextLevel;
                return JsonOk(retVal);
            }
            catch (UserNotFoundException)
            {
                m_AuthenticationManager.SignOut();
                return new HttpUnauthorizedResult();
            }

        }

        public ActionResult ChangeNotification(EmailSend subscribe)
        {
            var command = new UpdateUserEmailSubscribeCommand(User.GetUserId(), subscribe);
            ZboxWriteService.UpdateUserEmailSettings(command);
            return JsonOk();
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


        [HttpGet, ZboxAuthorize]
        [ActionName("Updates")]
        public async Task<ActionResult> UpdatesAsync()
        {
            var model = await ZboxReadService.GetUpdatesAsync(new QueryBase(User.GetUserId()));
            return JsonOk(model.Select(s => new
            {
                s.AnswerId,
                s.BoxId,
                s.QuestionId
            }));
        }
    }
}
