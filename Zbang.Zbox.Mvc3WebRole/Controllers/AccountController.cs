using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Profile;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Mvc3WebRole.Attributes;
using Zbang.Zbox.Mvc3WebRole.Controllers.Resources;
using Zbang.Zbox.Mvc3WebRole.Helpers;
using Zbang.Zbox.Mvc3WebRole.Models;
using Zbang.Zbox.Mvc3WebRole.Models.Account;
using Zbang.Zbox.Mvc3WebRole.Models.Account.AccountSettings;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs.UserDtos;
using Zbang.Zbox.ViewModel.Queries;
using mail = Zbang.Zbox.Infrastructure.Mail.Parameters;


namespace Zbang.Zbox.Mvc3WebRole.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IMailManager m_MailManager;
        private readonly IMembershipService m_MembershipService;
        private readonly IFacebookAuthenticationService m_FacebookService;

        public AccountController(IMailManager mailManage,
            IMembershipService membershipService,
            IFacebookAuthenticationService facebookService,
            IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            IShortCodesCache shortToLongCache,
            IFormsAuthenticationService formsAuthenticationService)
            : base(zboxWriteService, zboxReadService, shortToLongCache, formsAuthenticationService)
        {
            m_MailManager = mailManage;
            m_MembershipService = membershipService;
            m_FacebookService = facebookService;
        }


        //const string SessionUserDataKey = "UserData";

        //Do not output cache
        public ActionResult Index(string language, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            ViewBag.returnUrl = returnUrl;
            return View(new LogOnRegister { LogOn = new LogOn { RememberMe = true }, Register = new Register() });
        }

        public ActionResult LogOn(string language, string returnUrl)
        {
            return RedirectToAction("Index", new { language = language, returnUrl = returnUrl });
        }
        public ActionResult Register(string language, string returnUrl)
        {
            return RedirectToAction("Index", new { language = language, returnUrl = returnUrl });
        }

        public ActionResult ChangeLanguage(string language)
        {
            if (!Languages.CheckIfLanguageIsSupported(language))
            {
                return RedirectToAction("Index");
            }
            //ChangeLanguageFromSession(language);
            //RemoveLanguageFromSession();

            return RedirectToAction("Index", new { language = language });
        }

        #region Login
        [HttpPost]
        public ActionResult FacebookLogin(string token, string returnUrl)
        {
            try
            {
                LogInUserDto user;
                //bool isFirstTime = false;
                var facebookUserData = m_FacebookService.FacebookLogIn(token);
                if (facebookUserData == null)
                {
                    return Json(new JsonResponse(false, new { error = AccountControllerResources.FacebookGetDataError }));
                }
                try
                {
                    var query = new GetUserByFacebookQuery(facebookUserData.id);
                    user = m_ZboxReadService.GetUserDetailsByFacebookId(query);
                }
                catch (UserNotFoundException)
                {
                    var command = new CreateFacebookUserCommand(facebookUserData.id, facebookUserData.email, facebookUserData.name,
                        facebookUserData.Image, facebookUserData.LargeImage);
                    var commandResult = m_ZboxWriteService.CreateUser(command) as CreateFacebookUserCommandResult;
                    user = new LogInUserDto
                    {
                        Id = commandResult.User.Id,
                        Culture = commandResult.User.Culture,
                        ImageUrl = facebookUserData.Image,
                        Name = facebookUserData.name,
                        Uid = commandResult.User.Uid,
                        UniversityId = commandResult.User.University.Id
                    };
                    //isFirstTime = true;
                }
                //AddPageFriendToUser(from, user.Id);


                m_FormsAuthenticationService.SignIn(user.Id, false, new UserDetail(user.Name, user.Culture, user.ImageUrl, user.Uid, user.UniversityId));
                //if (isFirstTime)
                //{
                //    return Json(new JsonResponse(true, new { url = Url.Action("AccountSettings") }));
                //}
                return Json(new JsonResponse(true));
            }
            catch (WebException)
            {
                return Json(new JsonResponse(false, new { error = AccountControllerResources.FacebookGetDataError }));
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOn(LogOn model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.LogOn = true;
                return View("Index", new LogOnRegister { LogOn = model, Register = new Register() });
            }
            Guid membershipUserId;
            try
            {
                if (m_MembershipService.ValidateUser(model.Email, model.Password, out membershipUserId))
                {
                    try
                    {
                        var query = new GetUserByMembershipQuery(membershipUserId);
                        var result = m_ZboxReadService.GetUserDetailsByMembershipId(query);
                        m_FormsAuthenticationService.SignIn(result.Id, model.RememberMe,
                            new UserDetail(result.Name, result.Culture, result.ImageUrl, result.Uid, result.UniversityId));
                        return RedirectToLocal(returnUrl);
                    }
                    catch (UserNotFoundException)
                    {
                        m_MembershipService.DeleteUser(model.Email);
                        ModelState.AddModelError(string.Empty, AccountControllerResources.LogonError);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, AccountControllerResources.LogonError);
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("LogOn model : {0} ", model), ex);
                ModelState.AddModelError(string.Empty, AccountControllerResources.LogonError);
            }
            ViewBag.LogOn = true;
            return View("Index", new LogOnRegister { LogOn = model, Register = new Register() });

            //return Json(new JsonResponse(false, new { View = RenderRazorViewToString("LogOn", model) }));
        }

        //[NonAction]
        //private void AddPageFriendToUser(AuthSource? source, long userId)
        //{
        //    if (source.HasValue && source.Value == AuthSource.BankBB)
        //    {
        //        var BeitBerlAgudatStudentID = Convert.ToInt64(Zbang.Zbox.Infrastructure.Extensions.ConfigFetcher.Fetch("BeitBerlAgudatStudentID"));

        //        var command = new AddFriendCommand(userId, BeitBerlAgudatStudentID);
        //        m_ZboxWriteService.AddAFriend(command);
        //    }
        //}
        [NonAction]
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }


        public ActionResult LogOff()
        {
            Session.Abandon(); // remove the session cookie from user computer. wont continue session if user log in with a diffrent id.            
            m_FormsAuthenticationService.SignOut();
            return Redirect(FormsAuthentication.LoginUrl);// RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Register model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var userid = Guid.NewGuid().ToString();
                try
                {
                    Guid userProviderKey;
                    MembershipCreateStatus createStatus = m_MembershipService.CreateUser(userid, model.Password, model.NewEmail, out userProviderKey);
                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        CreateUserCommand command = new CreateMembershipUserCommand(userProviderKey, model.NewEmail, model.NewUserName, string.Empty);
                        var result = m_ZboxWriteService.CreateUser(command);
                        //Sign In

                        m_FormsAuthenticationService.SignIn(result.User.Id, false,
                            new UserDetail(result.User.Name, result.User.Culture, result.User.Image, result.User.Uid, result.User.University.Id));
                        //SendVerificationEmail(result.User.Id);

                        //Need to redirect to verify account
                        //AddPageFriendToUser(from, result.User.Id);
                        return RedirectToLocal(returnUrl);
                        //return Json(new JsonResponse(true));

                    }
                    ModelState.AddModelError(string.Empty, AccountValidation.ErrorCodeToString(createStatus));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }

            }
            return View("Index", new LogOnRegister { LogOn = new LogOn(), Register = model });
            //return Json(new JsonResponse(false, new { View = RenderRazorViewToString("Register", model) }));
        }
        #endregion

        #region AccountSettings
        [ZboxAuthorize]
        public ActionResult Settings()
        {
            var userId = GetUserId();
            var query = new GetUserDetailsQuery(userId);

            var user = m_ZboxReadService.GetUserAccountDetails(query);
            return View("Settings", user);
        }

        const string sessionKey = "UserVerificationCode";
        [HttpPost]
        [Ajax]
        [ZboxAuthorize]
        public ActionResult EnterCode(long? code)
        {
            if (!code.HasValue)
            {
                return Json(new JsonResponse(false, AccountControllerResources.ChangeEmailCodeError));
            }
            var model = Session[sessionKey] as ChangeMail;
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

            Session.Remove(sessionKey);
            return Json(new JsonResponse(true, model.Email));
        }

        [HttpPost, Ajax]
        [ZboxAuthorize]
        public ActionResult ChangeEmail(ChangeMail model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, base.GetModelStateErrors()));
            }
            try
            {
                var rand = new Random();
                var generatedCode = rand.Next(10000, 99999);
                model.Code = generatedCode;
                Session[sessionKey] = model;
                var mailParams = new mail.ChangeEmail(generatedCode);
                m_MailManager.SendEmail(mailParams, model.Email);
                return Json(new JsonResponse(true, new { code = true }));
            }
            catch (ArgumentException ex)
            {
                return Json(new JsonResponse(false, new { error = ex.Message }));
            }
        }

        [HttpPost]
        [ZboxAuthorize]
        //[ValidateAntiForgeryToken]
        public ActionResult ChangeProfile(Profile model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new JsonResponse(false, new { error = GetModelStateErrors() }));
                }
                var id = GetUserId();
                var profilePics = new ProfileImages(model.Image, model.LargeImage);

                var command = new UpdateUserProfileCommand(id, model.Name, profilePics.Image,
                    profilePics.LargeImage,
                    model.University);
                m_ZboxWriteService.UpdateUserProfile(command);

                m_FormsAuthenticationService.ChangeNameAndEmail(model.Name, model.Image, command.UniversityId);


                return Json(new JsonResponse(true));
            }
            catch (UserNotFoundException)
            {
                return Json(new JsonResponse(false, new { error = "User doen't exists" }));
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

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string password;
                    bool result = m_MembershipService.ResetPassword(model.Email, out password);
                    if (result)
                    {
                        mail.CreateMailBase parameters = new mail.ForgotPassword(password);

                        m_MailManager.SendEmail(parameters, model.Email);
                        TempData["email"] = model.Email;
                        return RedirectToAction("EmailSent");
                    }
                }
                catch (UserNotFoundException)
                {
                    ModelState.AddModelError(string.Empty, AccountControllerResources.EmailDoesNotExists);
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError(string.Format("ForgotPassword email: {0}", model.Email), ex);
                    ModelState.AddModelError(string.Empty, AccountControllerResources.UnspecifiedError);
                }

            }

            return View(model);
        }

        public ActionResult EmailSent()
        {
            if (TempData["email"] == null)
            {
                return RedirectToAction("ForgotPassword");
            }
            return View(new ForgotPassword { Email = TempData["email"].ToString() });
        }

        //[Authorize]
        //public ActionResult VerifyAccount()
        //{
        //    try
        //    {
        //        var query = new GetUserDetailsQuery(GetUserId());
        //        m_ZboxReadService.GetUserDetails(query);
        //        var userData = m_FormsAuthenticationService.GetUserData();
        //        if (userData == null)
        //        {
        //            return View();
        //        }
        //        if (userData.Verified)
        //        {
        //            return RedirectToAction("Index", "Dashboard");

        //        }
        //        return View();

        //    }
        //    catch (UserNotFoundException)
        //    {
        //        return RedirectToAction("LogOff");
        //    }

        //}

        //[Authorize]
        //public ActionResult VerifyAccountMail(string email)
        //{
        //    if (string.IsNullOrEmpty(email))
        //    {
        //        return RedirectToAction("VerifyAccount");
        //    }
        //    try
        //    {
        //        var bytes = MachineKey.Decode(email, MachineKeyProtection.Validation);
        //        var useruId = Encoding.UTF8.GetString(bytes);
        //        var userId = m_ShortToLongCode.ShortCodeToLong(useruId, ShortCodesType.User);
        //        if (userId != GetUserId())
        //        {
        //            throw new ArgumentException("user log in is not with the right email");
        //        }
        //        var command = new VerifyEmailCommand(userId);

        //        m_ZboxWriteService.VerifyEmail(command);
        //        m_FormsAuthenticationService.VerifyEmail();

        //        if (HttpContext.Request.UserAgent.IndexOf("iPhone", StringComparison.OrdinalIgnoreCase) > 0 ||
        //            HttpContext.Request.UserAgent.IndexOf("iPad", StringComparison.OrdinalIgnoreCase) > 0)
        //        {
        //            return Redirect("multimicloud://" + email);
        //        }
        //        return RedirectToAction("Settings");
        //    }
        //    catch (UserNotFoundException ex)
        //    {
        //        TraceLog.WriteError("Verify account mail user not found email: " + email, ex);
        //        return RedirectToAction("VerifyAccount");
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("VerifyAccountMail email: " + email, ex);
        //        return RedirectToAction("VerifyAccount");
        //    }

        //}

        //[Authorize]
        //public ActionResult ResendEmailVerification()
        //{
        //    SendVerificationEmail(GetUserId());
        //    return RedirectToAction("VerifyAccount");
        //}



        //[Authorize]
        //[NonAction]
        //private void SendVerificationEmail(long userid)
        //{
        //    //TODO: add to url the type of machinekeyprotection to url
        //    var query = new GetUserDetailsQuery(userid);
        //    var result = m_ZboxReadService.GetUserDetails(query);
        //    //Construct Verification Mail Action Uri                                           

        //    var plaintextBytes = Encoding.UTF8.GetBytes(result.Uid);
        //    var hashId = MachineKey.Encode(plaintextBytes, MachineKeyProtection.Validation);

        //    //Send Verification Mail           
        //    var parameters = new mail.EmailVerification(hashId, result.Name);

        //    m_MailManager.SendEmail(parameters, result.Email);

        //}

        [ChildActionOnly]
        public ActionResult GetUserDetail2()
        {

            if (!User.Identity.IsAuthenticated)
            {
                return PartialView("_UserDetail2");
            }
            Zbang.Zbox.ViewModel.DTOs.UserDtos.UserDto userData;
            try
            {
                var userCookieData = m_FormsAuthenticationService.GetUserData();
                if (userCookieData == null)
                {
                    var query = new GetUserDetailsQuery(GetUserId());
                    userData = m_ZboxReadService.GetUserData(query);

                }
                else
                {
                    userData = new ViewModel.DTOs.UserDtos.UserDto
                    {
                        Image = userCookieData.ImageUrl,
                        Name = userCookieData.Name,
                        Uid = userCookieData.Uid

                    };
                }

                return PartialView("_UserDetail2", userData);
            }
            //Zbang.Zbox.ViewModel.DTOs.UserDtos.UserDto userData = Session[SessionUserDataKey] as Zbang.Zbox.ViewModel.DTOs.UserDtos.UserDto;


            //if (userData != null)
            //{
            //    return PartialView("_UserDetail2", userData);
            //}
            //try
            //{
            //    var query = new GetUserDetailsQuery(GetUserId());
            //    var userData = ZboxReadService.GetUserData(query);
            //    return PartialView("_UserDetail2", userData);
            //    //Session[SessionUserDataKey] = userData;
            //}
            catch (UserNotFoundException)
            {
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("GetUserDetail user" + User.Identity.Name, ex);
                //if (ControllerContext.ParentActionViewContext.RouteData.Values["controller"].ToString() == "Error")
                //{
                return new EmptyResult();
                //}
            }
            //return PartialView("_UserDetail2", userData);
        }

        [ChildActionOnly]
        public ActionResult GetUserDetail3()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return PartialView("_UserDetail3");
            }
            Zbang.Zbox.ViewModel.DTOs.UserDtos.UserDto userData;
            try
            {
                var userCookieData = m_FormsAuthenticationService.GetUserData();
                if (userCookieData == null)
                {
                    var query = new GetUserDetailsQuery(GetUserId());
                    userData = m_ZboxReadService.GetUserData(query);
                }
                else
                {
                    userData = new ViewModel.DTOs.UserDtos.UserDto
                    {
                        Image = userCookieData.ImageUrl,
                        Name = userCookieData.Name,
                        Uid = userCookieData.Uid

                    };
                }

                return PartialView("_UserDetail3", userData);
            }
            catch (UserNotFoundException)
            {
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("GetUserDetail user" + User.Identity.Name, ex);
                //if (ControllerContext.ParentActionViewContext.RouteData.Values["controller"].ToString() == "Error")
                //{
                return new EmptyResult();
                //}
            }
        }
        //[NonAction]
        //private void GenerateCookie(double usedSpace, double allocatedSize, string userName, string userPicture, string shortid)
        //{
        //    var cookie = new HttpCookie(UserDetailsCookieName);
        //    cookie["si"] = shortid;
        //    //cookie["us"] = usedSpace.ToString(CultureInfo.InvariantCulture);
        //    //cookie["as"] = allocatedSize.ToString(CultureInfo.InvariantCulture);
        //    cookie["un"] = Server.UrlEncode(userName);
        //    cookie["up"] = userPicture;
        //    cookie.Secure = true;
        //    cookie.Expires = DateTime.Now.AddMinutes(20);

        //    Response.Cookies.Add(cookie);
        //}
        //[NonAction]
        //private bool GetCookie(out UserData userData)
        //{

        //    userData = null;
        //    var cookie = Request.Cookies[UserDetailsCookieName];
        //    if (cookie == null)
        //    {
        //        return false;
        //    }
        //    var shortUid = cookie["si"];
        //    var userid = ShortToLongCode.ShortCodeToLong(shortUid, ShortCodesType.User);
        //    if (GetUserId(false) != userid)
        //    {
        //        return false;
        //    }
        //    try
        //    {
        //        var userName = Server.UrlDecode(cookie["un"]);
        //        var userPicture = cookie["up"];
        //        userData = new UserData(userName, userPicture, shortUid);
        //        return true;
        //    }
        //    catch
        //    {
        //        RemoveCookie(UserDetailsCookieName);
        //        return false;
        //    }
        //}
    }
}
