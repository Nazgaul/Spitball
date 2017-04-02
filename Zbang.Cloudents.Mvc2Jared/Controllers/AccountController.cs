using System;

using System.Linq;
using System.Web.Mvc;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Cloudents.Mvc2Jared.Models;
using System.Web;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.AspNet.Identity;

namespace Zbang.Cloudents.Mvc2Jared.Controllers
{

    public class AccountController : Controller
    {
        private readonly IAuthenticationManager m_AuthenticationManager;
        public AccountController(IAuthenticationManager authenticationManager)
        {
            m_AuthenticationManager = authenticationManager;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Login")]
        public JsonResult LogIn(LogOn model)
        {
            var authentication = HttpContext.GetOwinContext().Authentication;
            if (!ModelState.IsValid)
            {
                return Json(GetErrorFromModelState());
            }
            try
            {
                if (model.Email == "elton@cloudents.com" && model.Password == "eltonjon")
                {
                    m_AuthenticationManager.SignIn(
                    new AuthenticationProperties { IsPersistent = true },
                    new ClaimsIdentity(new[] { new Claim(
                       ClaimsIdentity.DefaultNameClaimType, model.Email)
                    },
                       DefaultAuthenticationTypes.ApplicationCookie));
                    return Json(new { success = true, payload= Url.Action("Page", "home") });
                }
                return Json(new {success=false,payload="invalid password Or user name" });
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"LogOn model : {model} ", ex);
                ModelState.AddModelError(string.Empty, "Logon Error");
            }
            return Json(new {success=false,payload= "error" });
            // return Json(GetErrorFromModelState());
        }
        protected string GetErrorFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage)).FirstOrDefault();
        }
    }
}