using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using DevTrends.MvcDonutCaching;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models.Account;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings;
using Zbang.Cloudents.SiteExtension;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Profile;
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
        private readonly Lazy<IQueueProvider> m_QueueProvider;
        private readonly Lazy<IEncryptObject> m_EncryptObject;
        private UserManager m_UserManager;

        public AccountController(
            Lazy<IFacebookService> facebookService,
            Lazy<IQueueProvider> queueProvider,
            Lazy<IEncryptObject> encryptObject)
        {
            m_FacebookService = facebookService;
            m_QueueProvider = queueProvider;
            m_EncryptObject = encryptObject;
        }

        public AccountController(
           Lazy<IFacebookService> facebookService,
           Lazy<IQueueProvider> queueProvider,
           Lazy<IEncryptObject> encryptObject,
           UserManager userManager
           )
        {
            m_FacebookService = facebookService;
            m_QueueProvider = queueProvider;
            m_EncryptObject = encryptObject;
            m_UserManager = userManager;
        }


        public UserManager UserManager
        {
            get
            {
                return m_UserManager ?? HttpContext.GetOwinContext().GetUserManager<UserManager>();
            }
            private set
            {
                m_UserManager = value;
            }
        }


        //[FlushHeader(PartialViewName = "_HomeHeader")]
        //issue with ie
        [DonutOutputCache(VaryByParam = "lang;invId", VaryByCustom = CustomCacheKeys.Lang,
            Duration = TimeConsts.Day,
            Location = OutputCacheLocation.Server, Order = 2)]
        [RedirectToMobile(Order = 1)]
        //[PreserveQueryString]
        public ActionResult Index(string lang, string invId)
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    return RedirectToAction("Index", "Dashboard");
            //}
            if (!string.IsNullOrEmpty(invId))
            {
                var guid = GuidEncoder.TryParseNullableGuid(invId);
                if (guid.HasValue)
                {
                    var h = new CookieHelper(HttpContext);
                    h.InjectCookie(Invite.CookieName, new Invite { InviteId = guid.Value });
                }
            }
            //if (lang != null && lang != Thread.CurrentThread.CurrentUICulture.Name)
            //{
            //    RouteData.Values.Remove("lang");
            //    return RedirectToAction("Index");
            //}
            ViewBag.title = Views.Account.Resources.HomeResources.Title;
            ViewBag.metaDescription = Views.Account.Resources.HomeResources.Description;
            return View("Empty");
        }

        [HttpGet]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult IndexPartial()
        {
            return PartialView("Index2");
        }


        #region Login
        [HttpPost]
        [RequireHttps]
        public async Task<JsonResult> FacebookLogin(FacebookLogIn model, string returnUrl)
        {
            try
            {
                var cookie = new CookieHelper(HttpContext);
                var isNew = false;
                var facebookUserData = await m_FacebookService.Value.FacebookLogIn(model.Token);
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
                        facebookUserData.Image, facebookUserData.LargeImage, model.UniversityId,
                        facebookUserData.first_name,
                        facebookUserData.middle_name,
                        facebookUserData.last_name,
                        facebookUserData.GetGender(),
                        false, facebookUserData.locale, invId, model.BoxId, false);
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
                    isNew = true;
                }
                SiteExtension.UserLanguage.InsertCookie(user.Culture, HttpContext);

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


                AuthenticationManager.SignIn(identity);
                cookie.RemoveCookie(Invite.CookieName);
                return JsonOk(new { isnew = isNew, url = Url.RouteUrl("LibraryDesktop", new { returnUrl = CheckIfToLocal(returnUrl), @new = "true" }) });
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
        //[ValidateAntiForgeryToken]
        public async Task<JsonResult> LogIn(
            [ModelBinder(typeof(TrimModelBinder))]LogOn model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetModelStateErrors());
            }
            try
            {
                var query = new GetUserByEmailQuery(model.Email);
                var tSystemData = ZboxReadService.GetUserDetailsByEmail(query);
                var tUserIdentity = UserManager.FindByEmailAsync(model.Email);

                await Task.WhenAll(tSystemData, tUserIdentity);

                var user = tUserIdentity.Result;
                var systemUser = tSystemData.Result;
                if (user == null || systemUser == null)
                {
                    ModelState.AddModelError(string.Empty, "user does not exists");
                    return JsonError(GetModelStateErrors());
                }


                var loginStatus = await UserManager.CheckPasswordAsync(user, model.Password);

                if (loginStatus)
                {
                    var identity = await user.GenerateUserIdentityAsync(UserManager, systemUser.Id,
                        systemUser.UniversityId, systemUser.UniversityData);
                    AuthenticationManager.SignIn(identity);
                    var cookie = new CookieHelper(HttpContext);
                    cookie.RemoveCookie(Invite.CookieName);
                    SiteExtension.UserLanguage.InsertCookie(systemUser.Culture, HttpContext);

                    var url = systemUser.UniversityId.HasValue
                        ? Url.Action("Index", "Dashboard")
                        : Url.Action("Choose", "Library");
                    return JsonOk(url);

                }
                ModelState.AddModelError(string.Empty, "password incorrect");

            }
            catch (UserNotFoundException)
            {
                ModelState.AddModelError(string.Empty, AccountControllerResources.LogonError);
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
            //if (Session != null)
            //Session.Abandon(); // remove the session cookie from user computer. wont continue session if user log in with a diffrent id.            
            AuthenticationManager.SignOut();
            return RedirectToAction("Index");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }



        [HttpPost]
        //     [ValidateAntiForgeryToken] 
        public async Task<JsonResult> Register([ModelBinder(typeof(TrimModelBinder))] Register model)
        {

            if (!ModelState.IsValid)
            {
                return JsonError(GetModelStateErrors());
            }
            try
            {
                var cookie = new CookieHelper(HttpContext);
                var inv = cookie.ReadCookie<Invite>(Invite.CookieName);
                // Guid userProviderKey;

                var user = new ApplicationUser
                {
                    UserName = model.NewEmail,
                    Email = model.NewEmail,
                    //    UniversityId = model.UniversityId,
                    //    UniversityData = model.UniversityId
                };
                var createStatus = await UserManager.CreateAsync(user, model.Password);

                //var createStatus = m_MembershipService.Value.CreateUser(userid, model.Password, model.NewEmail,
                //    out userProviderKey);
                if (createStatus.Succeeded)
                {

                    Guid? invId = null;
                    if (inv != null)
                    {
                        invId = inv.InviteId;
                    }
                    var lang = cookie.ReadCookie<string>(SiteExtension.UserLanguage.CookieName);
                    if (!Languages.CheckIfLanguageIsSupported(lang))
                    {
                        lang = Thread.CurrentThread.CurrentCulture.Name;
                    }
                    CreateUserCommand command = new CreateMembershipUserCommand(Guid.Parse(user.Id),
                        model.NewEmail, model.UniversityId,
                        model.FirstName, model.LastName,
                        !model.IsMale.HasValue || model.IsMale.Value,
                        model.MarketEmail, lang, invId, model.BoxId, false);
                    var result = await ZboxWriteService.CreateUserAsync(command);
                    SiteExtension.UserLanguage.InsertCookie(result.User.Culture, HttpContext);

                    var identity = await user.GenerateUserIdentityAsync(UserManager, result.User.Id, result.UniversityId,
                         result.UniversityData);

                    AuthenticationManager.SignIn(identity);


                    cookie.RemoveCookie(Invite.CookieName);
                    return
                        JsonOk(
                            Url.RouteUrl("LibraryDesktop", new { returnUrl = CheckIfToLocal(model.ReturnUrl), @new = "true" }));

                }
                foreach (var error in createStatus.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
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


        //[ZboxAuthorize, NoUniversity]
        //public ActionResult Settings()
        //{
        //    return View("Empty");
        //}

        [ZboxAuthorize, NoUniversity]
        public JsonResult SettingsData()
        {
            var userId = User.GetUserId();
            var query = new GetUserDetailsQuery(userId);

            var user = ZboxReadService.GetUserAccountDetails(query);
            return JsonOk(user);
        }

        [DonutOutputCache(CacheProfile = "PartialPage")]
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
            var httpCookie = new CookieHelper(HttpContext);
            var model = httpCookie.ReadCookie<ChangeMail>(SessionKey);
            //var model = TempData[SessionKey] as ChangeMail;
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
                ZboxWriteService.UpdateUserEmail(command);
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
            httpCookie.RemoveCookie(SessionKey);
            //Session.Remove(SessionKey);
            return JsonOk(model.Email);
        }

        [HttpPost]
        [ZboxAuthorize]
        public async Task<JsonResult> ChangeEmail(ChangeMail model)
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
                model.TimeOfExpire = DateTime.UtcNow.AddHours(3);
                var httpCookie = new CookieHelper(HttpContext);
                httpCookie.InjectCookie(SessionKey, model);
                //TempData[SessionKey] = model;
                //Session[SessionKey] = model;

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

                //FormsAuthenticationService.ChangeUniversity(command.UniversityId,
                //command.UniversityDataId);


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


                AuthenticationManager.SignIn(user);



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
        public ActionResult ChangeLanguage(Models.Account.Settings.UserLanguage model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetModelStateErrors());
            }
            var id = User.GetUserId();
            var command = new UpdateUserLanguageCommand(id, model.Language);
            ZboxWriteService.UpdateUserLanguage(command);
            SiteExtension.UserLanguage.InsertCookie(model.Language, HttpContext);
            return JsonOk();
        }

        [HttpPost]
        public JsonResult ChangeLocale(string language)
        {
            SiteExtension.UserLanguage.InsertCookie(language, HttpContext);
            return JsonOk();
        }



        #region passwordReset
        //private const string SessionResetPassword = "SResetPassword";
        // ;
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

        [HttpPost, System.Web.Mvc.ValidateAntiForgeryToken]
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
                //Guid membershipUserId;
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email", AccountControllerResources.EmailDoesNotExists);
                    return View(model);
                }
                var query = new GetUserByMembershipQuery(Guid.Parse(user.Id));
                var tResult = ZboxReadService.GetUserDetailsByMembershipId(query);
                var tLinkData = UserManager.GeneratePasswordResetTokenAsync(user.Id);
                await Task.WhenAll(tResult, tLinkData);

                var code = RandomString(10);


                var data = new ForgotPasswordLinkData(Guid.Parse(user.Id), 1, tLinkData.Result);

                var linkData = EncryptElement(data);
                //Session[SessionResetPassword] = data;
                await m_QueueProvider.Value.InsertMessageToMailNewAsync(new ForgotPasswordData2(code, tLinkData.Result, tResult.Result.Name.Split(' ')[0], model.Email, tResult.Result.Culture));

                TempData["key"] = Crypto.HashPassword(code);

                return RedirectToAction("Confirmation", new { @continue = linkData });

            }

            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("ForgotPassword email: {0}", model.Email), ex);
                ModelState.AddModelError(string.Empty, AccountControllerResources.UnspecifiedError);
            }



            return View(model);
        }

        [HttpGet, NoCache]
        public ActionResult Confirmation(string @continue)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            if (TempData["key"] == null)
            {
                return RedirectToAction("ResetPassword");
            }
            //var userData = Session[SessionResetPassword] as ForgotPasswordLinkData;
            if (@continue == null)
            {
                return RedirectToAction("ResetPassword");
            }
            return View(new Confirmation { Key = TempData["key"].ToString() });
        }

        [HttpPost, System.Web.Mvc.ValidateAntiForgeryToken]
        [RequireHttps]
        public ActionResult Confirmation([ModelBinder(typeof(TrimModelBinder))] Confirmation model,
            string @continue)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userData = UnEncryptElement(@continue);//  Session[SessionResetPassword] as ForgotPasswordLinkData;

            if (userData == null)
            {
                return RedirectToAction("ResetPassword");
            }
            if (Crypto.VerifyHashedPassword(model.Key, model.Code))
            {
                userData.Step = 2;
                var key = EncryptElement(userData);
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
            var data = UnEncryptElement(key);
            if (data == null)
            {
                return RedirectToAction("index");
            }
            return View(new NewPassword());
        }

        [HttpPost]
        [System.Web.Mvc.ValidateAntiForgeryToken]
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
            var data = UnEncryptElement(key);
            if (data == null)
            {
                return RedirectToAction("index");
            }
            var result = await UserManager.ResetPasswordAsync(data.MembershipUserId.ToString(), data.Hash, model.Password);

            if (result.Succeeded)
            {
                var query = new GetUserByMembershipQuery(data.MembershipUserId);
                var tSystemUser = ZboxReadService.GetUserDetailsByMembershipId(query);

                var tUser = UserManager.FindByIdAsync(data.MembershipUserId.ToString());
                await Task.WhenAll(tSystemUser, tUser);

                var identity = await tUser.Result.GenerateUserIdentityAsync(UserManager, tSystemUser.Result.Id,
                       tSystemUser.Result.UniversityId, tSystemUser.Result.UniversityData);
                AuthenticationManager.SignIn(identity);
                return RedirectToAction("Index", "Dashboard");
            }
            ModelState.AddModelError(string.Empty, "something went wrong, try again later");
            return View(model);
            //m_MembershipService.Value.ResetPassword(data.MembershipUserId, model.Password);

            //var query = new GetUserByMembershipQuery(data.MembershipUserId);
            //var result = await ZboxReadService.GetUserDetailsByMembershipId(query);
            //SiteExtension.UserLanguage.InsertCookie(result.Culture, HttpContext);
            //FormsAuthenticationService.SignIn(result.Id, false,
            //    new UserDetail(
            //        result.UniversityId,
            //        result.UniversityData
            //        ));



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

        [ChildActionOnly]
        public ActionResult GetUserDetail3()
        {
            const string userDetailView = "_UserDetailStatic";
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


        [HttpPost, ZboxAuthorize]
        public JsonResult FirstTime(FirstTime firstTime)
        {
            var userid = User.GetUserId();
            var command = new UpdateUserFirstTimeStatusCommand(firstTime, userid);
            ZboxWriteService.UpdateUserFirstTimeStatus(command);

            return JsonOk();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "PartialCache")]
        public ActionResult CongratsPartial()
        {
            try
            {
                return PartialView("_Congrats");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("_Congrats", ex);
                return Json(new JsonResponse(false));
            }
        }

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
