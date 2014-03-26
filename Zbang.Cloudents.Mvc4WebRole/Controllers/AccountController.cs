using Cobisi.EmailVerify;
using DevTrends.MvcDonutCaching;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models.Account;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Profile;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs.UserDtos;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{

    public class AccountController : BaseController
    {
        private readonly Lazy<IMembershipService> m_MembershipService;
        private readonly Lazy<IFacebookAuthenticationService> m_FacebookService;
        private readonly Lazy<IUserProfile> m_UserProfile;
        private readonly Lazy<IQueueProvider> m_QueueProvider;
        // private readonly Lazy<IEmailVerfication> m_EmailVerification;
        private readonly Lazy<IEncryptObject> m_EncryptObject;


        public AccountController(
            Lazy<IMembershipService> membershipService,
            Lazy<IFacebookAuthenticationService> facebookService,
            IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            IFormsAuthenticationService formsAuthenticationService,
            Lazy<IUserProfile> userProfile,
            Lazy<IQueueProvider> queueProvider,
            //Lazy<IEmailVerfication> emailVerification,
            Lazy<IEncryptObject> encryptObject)
            : base(zboxWriteService, zboxReadService,
            formsAuthenticationService)
        {
            m_MembershipService = membershipService;
            m_FacebookService = facebookService;
            m_UserProfile = userProfile;
            m_QueueProvider = queueProvider;
            //m_EmailVerification = emailVerification;
            m_EncryptObject = encryptObject;
        }


        //[FlushHeader(PartialViewName = "_HomeHeader")]
        //issue with ie
        //donut output cache doesnt support route
        //[OutputCache(VaryByParam = "universityId;lang", VaryByCustom = CustomCacheKeys.Auth + ";"
        //    + CustomCacheKeys.Lang + ";"
        //    + CustomCacheKeys.Mobile, Duration = TimeConsts.Hour, Location = System.Web.UI.OutputCacheLocation.Server, Order = 2)]
        [CompressFilter(Order = 1)]
        [Route("Account/{lang:regex(^[A-Za-z]{2}-[A-Za-z]{2}$)?}")]
        public ActionResult Index(long? universityId, string lang)
        {

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            if (!string.IsNullOrEmpty(lang))
            {
                ChangeThreadLanguage(lang);
            }
            ViewBag.universityId = universityId;
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
        [Ajax]
        [RequireHttps]
        public async Task<ActionResult> FacebookLogin(string token, long? universityId)
        {
            try
            {
                LogInUserDto user;
                var isNew = false;
                var facebookUserData = await m_FacebookService.Value.FacebookLogIn(token);
                if (facebookUserData == null)
                {
                    return Json(new JsonResponse(false, new { error = AccountControllerResources.FacebookGetDataError }));
                }
                try
                {
                    var query = new GetUserByFacebookQuery(facebookUserData.id);
                    user = m_ZboxReadService.GetUserDetailsByFacebookId(query);
                    //try
                    //{
                    //    var command = new UpdateUserEmailCommand(user.Uid, facebookUserData.email, true);
                    //    m_ZboxWriteService.UpdateUserEmail(command);
                    //}
                    //catch
                    //{ }
                }
                catch (UserNotFoundException)
                {
                    var command = new CreateFacebookUserCommand(facebookUserData.id, facebookUserData.email,
                        facebookUserData.Image, facebookUserData.LargeImage, universityId,
                        facebookUserData.first_name,
                        facebookUserData.middle_name,
                        facebookUserData.last_name,
                        facebookUserData.GetGender());
                    var commandResult = m_ZboxWriteService.CreateUser(command) as CreateFacebookUserCommandResult;
                    user = new LogInUserDto
                    {
                        Uid = commandResult.User.Id,
                        Culture = commandResult.User.Culture,
                        Image = facebookUserData.Image,
                        Name = facebookUserData.name,
                        UniversityId = commandResult.UniversityId,
                        UniversityWrapperId = commandResult.UniversityWrapperId,
                        Score = commandResult.User.Reputation
                    };
                    isNew = true;
                    //TempData[TempDataNameUserRegisterFirstTime] = true;
                }


                m_FormsAuthenticationService.SignIn(user.Uid, false, new UserDetail(
                    user.Culture,
                    user.UniversityId,
                    user.UniversityWrapperId));
                TempData[UserProfile.UserDetail] = new UserDetailDto(user);
                return Json(new JsonResponse(true, new { isnew = isNew }));
            }
            catch (ArgumentException)
            {
                return Json(new JsonResponse(false, new { error = AccountControllerResources.FacebookGetDataError }));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("FacebookLogin", ex);
                return Json(new JsonResponse(false, new { error = AccountControllerResources.FacebookGetDataError }));

            }
        }



        [HttpPost]
        [Ajax]
        [ValidateAntiForgeryToken]
        public JsonResult LogIn([ModelBinder(typeof(TrimModelBinder))]LogOn model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
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
                        var result = m_ZboxReadService.GetUserDetailsByMembershipId(query);

                        m_FormsAuthenticationService.SignIn(result.Uid, model.RememberMe,
                            new UserDetail(
                                result.Culture,
                                result.UniversityId,
                                result.UniversityWrapperId));
                        TempData[UserProfile.UserDetail] = new UserDetailDto(result);
                        //  var url = result.UniversityId.HasValue ? Url.Action("Index", "Dashboard") : Url.Action("Choose", "Library");
                        return Json(new JsonResponse(true/*, url*/));
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
            return Json(new JsonResponse(false, GetModelStateErrors()));
        }


        //[NonAction]
        //private ActionResult RedirectToLocal(string returnUrl, bool isNew = false)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    return isNew ? RedirectToAction("Index", "Home").AddFragment("Library") : RedirectToAction("Index", "Home");
        //}


        public ActionResult LogOff()
        {
            Session.Abandon(); // remove the session cookie from user computer. wont continue session if user log in with a diffrent id.            
            m_FormsAuthenticationService.SignOut();
            return Redirect(FormsAuthentication.LoginUrl.ToLower());// RedirectToAction("Index");
        }

        [HttpPost]
        [Ajax]
        public JsonResult CheckEmail(Register model)
        {
            try
            {
                //  var retVal = await m_EmailVerification.Value.VerifyEmailAsync(model.NewEmail);
                return Json(true);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On check email", ex);
                return Json(true);
            }
        }

        [HttpPost]
        [Ajax]
        [ValidateAntiForgeryToken]
        public ActionResult Register([ModelBinder(typeof(TrimModelBinder))] Register model, long? universityId)
        {

            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, base.GetModelStateErrors()));
            }
            //var retVal = await m_EmailVerification.Value.VerifyEmailAsync(model.NewEmail);
            //if (!retVal)
            //{
            //    ModelState.AddModelError("NewEmail", Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources.RegisterResources.EmailNotValid);
            //    return Json(new JsonResponse(false, base.GetModelStateErrors()));
            //}
            var userid = Guid.NewGuid().ToString();
            try
            {
                Guid userProviderKey;
                var createStatus = m_MembershipService.Value.CreateUser(userid, model.Password, model.NewEmail, out userProviderKey);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    CreateUserCommand command = new CreateMembershipUserCommand(userProviderKey,
                        model.NewEmail, universityId, model.FirstName, string.Empty, model.LastName, model.Sex);
                    var result = m_ZboxWriteService.CreateUser(command);

                    m_FormsAuthenticationService.SignIn(result.User.Id, false,
                        new UserDetail(
                            result.User.Culture,
                            result.UniversityId, result.UniversityWrapperId));
                    return Json(new JsonResponse(true, Url.Action("Index", "Library")));

                }
                ModelState.AddModelError(string.Empty, AccountValidation.ErrorCodeToString(createStatus));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return Json(new JsonResponse(false, base.GetModelStateErrors()));
        }
        #endregion

        #region AccountSettings
        [ZboxAuthorize, UserNavNWelcome, NoUniversity, CompressFilter]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        [RequireHttps]
        public ActionResult Settings()
        {
            var userId = GetUserId();
            var query = new GetUserDetailsQuery(userId);

            var user = m_ZboxReadService.GetUserAccountDetails(query);
            if (Request.IsAjaxRequest())
            {
                return PartialView(user);
            }
            return View(user);
        }

        const string SessionKey = "UserVerificationCode";
        [HttpPost]
        [Ajax]
        [ZboxAuthorize]
        public ActionResult EnterCode(long? code)
        {
            if (!code.HasValue)
            {
                return Json(new JsonResponse(false, AccountControllerResources.ChangeEmailCodeError));
            }
            var model = Session[SessionKey] as ChangeMail;
            if (model == null)
            {
                return Json(new JsonResponse(false, AccountControllerResources.ChangeEmailCodeError));
            }
            if (code != model.Code)
            {
                return Json(new JsonResponse(false, AccountControllerResources.ChangeEmailCodeError));
            }
            var id = GetUserId();
            try
            {
                var command = new UpdateUserEmailCommand(id, model.Email);
                m_ZboxWriteService.UpdateUserEmail(command);
            }
            catch (Exception ex)
            {
                return Json(new JsonResponse(false, ex.Message));
            }

            Session.Remove(SessionKey);
            return Json(new JsonResponse(true, model.Email));
        }

        [HttpPost, Ajax]
        [ZboxAuthorize]
        public ActionResult ChangeEmail(ChangeMail model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            try
            {
                var rand = new Random();
                var generatedCode = rand.Next(10000, 99999);
                model.Code = generatedCode;
                Session[SessionKey] = model;

                m_QueueProvider.Value.InsertMessageToMailNew(new ChangeEmailData(generatedCode.ToString(), model.Email, System.Threading.Thread.CurrentThread.CurrentCulture.Name));
                return Json(new JsonResponse(true, new { code = true }));
            }
            catch (ArgumentException ex)
            {
                return Json(new JsonResponse(false, new { error = ex.Message }));
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
                    return Json(new JsonResponse(false, new { error = GetModelStateErrors() }));
                }
                var id = GetUserId();
                var profilePics = new ProfileImages(model.Image, model.LargeImage);

                var command = new UpdateUserProfileCommand(id, profilePics.Image,
                    profilePics.LargeImage, model.FirstName, model.MiddleName, model.LastName);
                m_ZboxWriteService.UpdateUserProfile(command);
                return Json(new JsonResponse(true));
            }
            catch (UserNotFoundException)
            {
                return Json(new JsonResponse(false, new { error = "User doen't exists" }));
            }
        }
        [HttpPost, Ajax, ZboxAuthorize]
        public async Task<ActionResult> UpdateUniversity(University model)
        {
            var retVal = await m_ZboxReadService.GetDepartmentList(model.UniversityId);
            if (retVal.Count() != 0 && !model.DepartmentId.HasValue)
            {
                return RedirectToAction("SelectDepartment", "Library", new { universityId = model.UniversityId });
            }
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, new { error = GetModelStateErrors() }));
            }

            try
            {
                var id = GetUserId();
                var command = new UpdateUserUniversityCommand(model.UniversityId, id, model.DepartmentId, model.Code, model.GroupNumber, model.RegisterNumber);
                m_ZboxWriteService.UpdateUserUniversity(command);
                m_FormsAuthenticationService.ChangeUniversity(command.UniversityId, command.UniversityWrapperId);
                return this.CdJson(new JsonResponse(true, new { redirect = Url.Action("Index", "Library") }));
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError("Code", Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources.AccountSettingsResources.CodeIncorrect);
                return this.CdJson(new JsonResponse(false, GetModelStateErrors()));

            }
        }


        [HttpPost]
        [ZboxAuthorize]
        public ActionResult ChangePassword(Password model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            var id = GetUserId();
            var command = new UpdateUserPasswordCommand(id, model.CurrentPassword, model.NewPassword);
            var commandResult = m_ZboxWriteService.UpdateUserPassword(command);
            return Json(new JsonResponse(!commandResult.HasErrors, commandResult.Error));
        }


        [HttpPost]
        [ZboxAuthorize]
        [RequireHttps]
        public ActionResult ChangeLanguage(UserLanguage model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            var id = GetUserId();
            var command = new UpdateUserLanguageCommand(id, model.Language);
            m_ZboxWriteService.UpdateUserLanguage(command);
            m_FormsAuthenticationService.ChangeLanguage(model.Language);
            return Json(new JsonResponse(true));
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
        [CompressFilter]
        public ActionResult ResetPassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        [NonAjax]
        public ActionResult ResetPassword([ModelBinder(typeof(TrimModelBinder))]ForgotPassword model)
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
                var result = m_ZboxReadService.GetUserDetailsByMembershipId(query);
                var code = RandomString(10);


                ForgotPasswordLinkData data = new ForgotPasswordLinkData(membershipUserId, 1);

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

        [NonAjax]
        [HttpGet, CacheFilter(Duration = 0)]
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
                return RedirectToAction("PasswordUpdate", new { key = key });
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
            var data = UncryptElement<ForgotPasswordLinkData>(key);
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
        public ActionResult PasswordUpdate([ModelBinder(typeof(TrimModelBinder))] NewPassword model, string key)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                return View(model);
            }
            var data = UncryptElement<ForgotPasswordLinkData>(key);
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
            var result = m_ZboxReadService.GetUserDetailsByMembershipId(query);
            Session.Abandon();
            m_FormsAuthenticationService.SignIn(result.Uid, false,
                new UserDetail(
                    result.Culture,
                    result.UniversityId,
                    result.UniversityWrapperId));

            return RedirectToAction("Index", "Dashboard");

        }


        [NonAction]
        private string RandomString(int Size)
        {
            var random = new Random();
            //string input = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string input = "0123456789";
            var chars = Enumerable.Range(0, Size)
                                   .Select(x => input[random.Next(0, input.Length)]);
            return new string(chars.ToArray());
            //return "12345";
        }
        [NonAction]
        private string CrypticElement<T>(T obj) where T : class
        {
            return m_EncryptObject.Value.EncryptElement<T>(obj, ResetPasswordCrypticPropose);

        }
        [NonAction]
        private T UncryptElement<T>(string str) where T : class
        {
            return m_EncryptObject.Value.DecryptElement<T>(str, ResetPasswordCrypticPropose);

        }
        #endregion



        [ChildActionOnly]
        public ActionResult GetUserDetail3()
        {
            const string userDatailView = "_UserDetail3";
            var urlBuilder = new UrlBuilder(HttpContext);
            if (User == null || !(User.Identity.IsAuthenticated))
            {
                return PartialView(userDatailView);
            }
            try
            {
                var userData = m_UserProfile.Value.GetUserData(ControllerContext);
                userData.Url = urlBuilder.BuildUserUrl(userData.Uid, userData.Name);
                return PartialView(userDatailView, userData);
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


        [HttpPost, ZboxAuthorize, Ajax]
        public JsonResult FirstTime(FirstTime firstTime)
        {
            var userid = GetUserId();
            var command = new UpdateUserFirstTimeStatusCommand(firstTime, userid);
            m_ZboxWriteService.UpdateUserFirstTimeStatus(command);

            return Json(new JsonResponse(true));
        }

    }


}
