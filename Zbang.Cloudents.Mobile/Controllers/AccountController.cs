﻿using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Microsoft.Owin.Security;
using Zbang.Cloudents.Mobile.Controllers.Resources;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Cloudents.Mobile.Helpers;
using Zbang.Cloudents.Mobile.Models.Account;
using Zbang.Cloudents.Mobile.Models.Account.Settings;
using Zbang.Cloudents.SiteExtension;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ViewModel.Dto.UserDtos;
using Zbang.Zbox.ViewModel.Queries;


namespace Zbang.Cloudents.Mobile.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class AccountController : BaseController
    {
        //private readonly Lazy<IMembershipService> m_MembershipService;
        private readonly Lazy<IFacebookService> m_FacebookService;
        private readonly Lazy<IQueueProvider> m_QueueProvider;
        private readonly Lazy<IEncryptObject> m_EncryptObject;
        private readonly ApplicationUserManager m_UserManager;
        private readonly IAuthenticationManager m_AuthenticationManager;

        // private const string InvId = "invId";
        public AccountController(
            //Lazy<IMembershipService> membershipService,
            Lazy<IFacebookService> facebookService,
            Lazy<IQueueProvider> queueProvider,
            ApplicationUserManager userManager,
            Lazy<IEncryptObject> encryptObject, IAuthenticationManager authenticationManager)
        {

            // m_MembershipService = membershipService;
            m_FacebookService = facebookService;
            m_QueueProvider = queueProvider;
            m_EncryptObject = encryptObject;
            m_UserManager = userManager;
            m_AuthenticationManager = authenticationManager;
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
                var query = new GetUserByFacebookQuery(facebookUserData.Id);
                LogInUserDto user = await ZboxReadService.GetUserDetailsByFacebookId(query);
                if (user == null)
                {

                    var inv = cookie.ReadCookie<Invite>(Invite.CookieName);
                    Guid? invId = null;
                    if (inv != null)
                    {
                        invId = inv.InviteId;
                    }
                    var command = new CreateFacebookUserCommand(facebookUserData.Id, facebookUserData.Email,
                         facebookUserData.LargeImage, universityId,
                        facebookUserData.First_name,

                        facebookUserData.Last_name,
                        facebookUserData.Locale, invId);
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
                SiteExtension.UserLanguage.InsertCookie(user.Culture, HttpContext);

                var identity = ApplicationUser.GenerateUserIdentity(user.Id, user.UniversityId, user.UniversityData);
                m_AuthenticationManager.SignIn(identity);

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
        //[Filters.ValidateAntiForgeryToken]
        public async Task<JsonResult> Login([ModelBinder(typeof(TrimModelBinder))]LogOn model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetModelStateErrors());
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
                    return JsonError(GetModelStateErrors());
                }
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, AccountValidation.ErrorCodeToString(AccountValidation.AccountErrors.InvalidEmail));
                    return JsonError(GetModelStateErrors());
                }
                //user.UserId = systemUser.Id;
                //user.UniversityId = systemUser.UniversityId;
                //user.UniversityData = systemUser.UniversityData;

                var loginStatus = await m_UserManager.CheckPasswordAsync(user, model.Password);
                if (loginStatus)
                {
                    var identity = await user.GenerateUserIdentityAsync(m_UserManager, systemUser.Id, systemUser.UniversityId,
                         systemUser.UniversityData);
                    m_AuthenticationManager.SignIn(identity);
                    var cookie = new CookieHelper(HttpContext);
                    cookie.RemoveCookie(Invite.CookieName);
                    SiteExtension.UserLanguage.InsertCookie(systemUser.Culture, HttpContext);
                    return JsonOk();
                }
                ModelState.AddModelError(string.Empty, AccountValidation.ErrorCodeToString(AccountValidation.AccountErrors.InvalidPassword));
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
            m_AuthenticationManager.SignOut();
            return Redirect("https://www.spitball.co"); //RedirectToRoute("accountLink");
        }



        [HttpPost]
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
                var user = new ApplicationUser
                {
                    UserName = model.NewEmail,
                    Email = model.NewEmail
                    //UniversityId = model.UniversityId,
                    //UniversityData = model.UniversityId
                };
                var createStatus = await m_UserManager.CreateAsync(user, model.Password);

                if (createStatus.Succeeded)
                {

                    Guid? invId = null;
                    if (inv != null)
                    {
                        invId = inv.InviteId;
                    }

                    CreateUserCommand command = new CreateMembershipUserCommand(Guid.Parse(user.Id),
                        model.NewEmail, model.UniversityId, model.FirstName, model.LastName,
                        CultureInfo.CurrentCulture.Name, invId);
                    var result = await ZboxWriteService.CreateUserAsync(command);
                    SiteExtension.UserLanguage.InsertCookie(result.User.Culture, HttpContext);

                    var identity = await user.GenerateUserIdentityAsync(m_UserManager, result.User.Id, result.UniversityId,
                         result.UniversityData);

                    m_AuthenticationManager.SignIn(identity);

                    cookie.RemoveCookie(Invite.CookieName);
                    return
                        JsonOk();

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
            if (needId != null /*&& string.IsNullOrEmpty(model.StudentId)*/)
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
                    null, null);
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
            SiteExtension.UserLanguage.InsertCookie(lang, HttpContext);
            return JsonOk();
        }


        //#endregion
        #region passwordReset
        //private const string SessionResetPassword = "SResetPassword";
        // ;
        [HttpGet]
        //issue with ie
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
                var query = new GetUserByEmailQuery(model.Email);
                //Guid membershipUserId;
                var tUser = m_UserManager.FindByEmailAsync(model.Email);
                var tResult = ZboxReadService.GetUserDetailsByEmail(query);

                await Task.WhenAll(tUser, tResult);
                if (tUser.Result == null && tResult.Result != null)
                {
                    ModelState.AddModelError("Email", "You have registered to Cloudents through Facebook -- go to the homepage and click on the Facebook button to register");
                    return View("ForgotPwd", model);
                }
                if (tUser.Result == null)
                {
                    ModelState.AddModelError("Email", AccountControllerResources.EmailDoesNotExists);
                    return View("ForgotPwd", model);
                }
                var user = tUser.Result;
                //var tResult = ZboxReadService.GetUserDetailsByMembershipId(query);
                var identitylinkData = await m_UserManager.GeneratePasswordResetTokenAsync(user.Id);


                var code = RandomString(10);


                var data = new ForgotPasswordLinkData(Guid.Parse(user.Id), 1, identitylinkData);

                var linkData = EncryptElement(data);
                //Session[SessionResetPassword] = data;
                await m_QueueProvider.Value.InsertMessageToMailNewAsync(new ForgotPasswordData2(code, linkData, tResult.Result.Name.Split(' ')[0], model.Email, tResult.Result.Culture));

                TempData["key"] = Crypto.HashPassword(code);

                return RedirectToAction("Confirmation", new { @continue = linkData });

            }

            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("ForgotPassword email: {0}", model.Email), ex);
                ModelState.AddModelError(string.Empty, AccountControllerResources.UnspecifiedError);
            }
            return View("ForgotPwd", model);

        }

        [HttpGet, NoCache]
        public ActionResult Confirmation(string @continue)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("dashboardLink");
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
            return View("CheckEmail", new Confirmation { Key = TempData["key"].ToString() });
        }

        [HttpPost]
        public ActionResult Confirmation([ModelBinder(typeof(TrimModelBinder))] Confirmation model, string @continue)
        {
            if (!ModelState.IsValid)
            {
                return View("CheckEmail", model);
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
            var data = UnEncryptElement(key);
            if (data == null)
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
            var data = UnEncryptElement(key);
            if (data == null)
            {
                return RedirectToRoute("accountLink");
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
                return RedirectToRoute("dashboardLink");
            }
            ModelState.AddModelError(string.Empty, "something went wrong, try again later");
            return View("ChoosePwd", model);


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


        [HttpGet]
        public async Task<JsonResult> Details()
        {
            string cookieToken, formToken;
            AntiForgery.GetTokens(null, out cookieToken, out formToken);
            var token = formToken;

            var cookieHelper = new CookieHelper(HttpContext);
            cookieHelper.InjectCookie(AntiForgeryConfig.CookieName, cookieToken);
            if (!User.Identity.IsAuthenticated)
            {
                return JsonOk(new { token, Culture = CultureInfo.CurrentCulture.Name });
            }
            var retVal = await ZboxReadService.GetUserDataAsync(new GetUserDetailsQuery(User.GetUserId()));
            retVal.Token = token;
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
                retVal.Token
            });

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
