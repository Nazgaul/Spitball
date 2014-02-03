using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.MvcWebRole.Helpers;
using Zbang.Zbox.MvcWebRole.Models;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Zbox.MvcWebRole.Controllers
{
    [HandleError]
    public class AccountController : Controller
    {
        //Fields

        private IZboxService m_ZboxService;
        private IMailManager m_MailManager;
        private IFormsAuthenticationService m_FormsAuthenticationService;
        private IMembershipService m_MembershipService;

        //Ctor
        public AccountController(IZboxService zboxService, IMailManager mailManager,
            IFormsAuthenticationService formsAuthenticationService, IMembershipService membershipService)
        {
            m_ZboxService = zboxService;
            m_MailManager = mailManager;


            m_FormsAuthenticationService = formsAuthenticationService;
            m_MembershipService = membershipService;
        }

        //Methods


        public ActionResult StartPage()
        {
            return View();
        }

        public ActionResult LogOn()
        {
            return View("startPage");
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                string userName = string.Empty, email = string.Empty;
                //string userName = model.UserName;
                if (!string.IsNullOrEmpty(model.UserName) && model.UserName.Contains("@"))
                    email = model.UserName;
                else
                    userName = model.UserName;

                if (m_MembershipService.ValidateUser(ref userName, email, model.Password.Trim()))
                {
                    m_FormsAuthenticationService.SignIn(userName, model.RememberMe);

                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }
            ViewData["ErrorLogOn"] = "Email or password is incorrect";
            //If we got this far, something failed, redisplay form

            return View("startPage", model);
        }

        public ActionResult LogOff()
        {
            m_FormsAuthenticationService.SignOut();

            return RedirectToAction("Index", "Home");
        }


        public ActionResult Register()
        {
            //ViewData["PasswordLength"] = m_MembershipService.MinPasswordLength;
            return View("startPage");
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = m_MembershipService.CreateUser(model.NewUserName, model.NewPassword, model.NewEmail);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    MembershipUser membershipUser = Membership.GetUser(model.NewUserName);

                    //Create Storage for user                    
                    CreateUserCommand command = new CreateUserCommand(membershipUser.Email);
                    CreateUserCommandResult result = m_ZboxService.CreateUser(command);

                    SendVerificationEmail(membershipUser);

                    //Sign In
                    m_FormsAuthenticationService.SignIn(model.NewUserName, false);

                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    //ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                    ViewData["ErrorRegister"] = AccountValidation.ErrorCodeToString(createStatus);
                }
            }
            else
            {

                var x = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage).FirstOrDefault();
                ViewData["ErrorRegister"] = x.ToString();

            }

            // If we got this far, something failed, redisplay form
            //ViewData["PasswordLength"] = m_MembershipService.MinPasswordLength;
            return View("startPage", model);
        }

        private void SendVerificationEmail(MembershipUser membershipUser)
        {

            //Construct Verification Mail Action Uri                                           
            string veriftMailUri = string.Format("{0}://{1}/Account/VerifyMail", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority);

            //Send Verification Mail           
            CreateMailParams parameters = new CreateMailParams()
            {
                EmailUserId = membershipUser.Email,
                UserName = membershipUser.UserName,
                VerificationUri = veriftMailUri
            };
            m_MailManager.SendEmail(parameters, CreateMailParams.EmailVerification, membershipUser.Email);
            
        }

        [Authorize]
        [HttpPost]
        public JsonResult ChangePassword(string oldPassword, string newPassword)
        {
            if (m_MembershipService.ChangePassword(User.Identity.Name, oldPassword, newPassword))
            {
                return this.Json(new JsonResponse(true));

            }
            else
            {
                return this.Json(new JsonResponse(false, "The current password is incorrect or the new password is invalid."));

            }
        }

        [HttpGet]
        public ActionResult VerifyMail(string EmailId)
        {
            VerifyEmailCommand command = new VerifyEmailCommand(EmailId);

            VerifyEmailCommandResult result = m_ZboxService.VerifyEmail(command);

            this.ViewData["VerificationResult"] = result.Verified ? "Verified." : "NOT Verified!";
            return View();
        }

        [Authorize]
        [HttpPost]
        public JsonResult ResendEmailVerification()
        {
            MembershipUser membershipUser = Membership.GetUser(this.User.Identity.Name);
            SendVerificationEmail(membershipUser);
            return this.Json(new JsonResponse(true));
        }

        [Authorize]
        [HttpGet]
        public JsonResult GetUserDertails()
        {
            GetUserDetailsQuery query = new GetUserDetailsQuery(ExtractUserID.GetUserEmailId());

            UserDto user = m_ZboxService.GetUserDetails(query);
            int usagePercentge = (int)((user.UsedSpace / user.AllocatedSize) * 100);
            if (usagePercentge < 0)
                usagePercentge = 0;
            var quotaFormatted = new { UsagePercentage = usagePercentge, TotalSize = user.AllocatedSize, Unit = "GB" };
            var IsVerified = user.IsEmailVerified;

            return this.Json(new JsonResponse(true, new { quota = quotaFormatted, IsVerified = IsVerified }), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ResetPassword(string UserDetail)
        {
            if (string.IsNullOrEmpty(UserDetail))
                throw new ArgumentException("email is null or empty", "email");
            //string userName = model.UserName;

            string password;
            string email;
            bool result = m_MembershipService.ResetPassword(UserDetail, out password, out email);
            if (result)
            {
                CreateMailParams parameters = new CreateMailParams()
                {
                    Password = password
                };
                m_MailManager.SendEmail(parameters, CreateMailParams.ForgotPassword, email);
            }

            return this.Json(new JsonResponse(result, null));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult KeepAlive()
        {
            //this.Session["KeepSessionAlive"] = DateTime.Now;

            return this.Json(new JsonResponse(true));
        }
    }
}
