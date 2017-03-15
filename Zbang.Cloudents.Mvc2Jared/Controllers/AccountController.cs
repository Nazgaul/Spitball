using System;

using System.Linq;
using System.Web.Mvc;
using Zbang.Zbox.Infrastructure.Trace;
using System.Threading.Tasks;
using Zbang.Cloudents.Mvc2Jared.Models;

namespace Zbang.Cloudents.Mvc2Jared.Controllers
{

    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<JsonResult> LogInAsync(LogOn model)
        {
            if (!ModelState.IsValid)
            {
                return Json(GetErrorFromModelState());
            }
            try
            {
                if (model.Email == "yifatbij@gmail.com" && model.Password == "123123")
                {
                    return Json(Url.Action("Page", "home"));
                }
                return Json("invalid password Or user name");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"LogOn model : {model} ", ex);
                ModelState.AddModelError(string.Empty, "Logon Error");
            }
            return Json("error");
            // return Json(GetErrorFromModelState());
        }
        protected string GetErrorFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage)).FirstOrDefault();
        }
    }
}