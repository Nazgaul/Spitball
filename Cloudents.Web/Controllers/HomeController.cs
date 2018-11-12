using Cloudents.Core.Entities.Db;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Cloudents.Web.Filters;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        internal const string Referral = "referral";


      

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [ServiceFilter(typeof(RedirectToOldSiteFilterAttribute))]
        public IActionResult Index(
           // [ModelBinder(typeof(CountryModelBinder))] string country,
            [FromHeader(Name = "User-Agent")] string userAgent,
            [FromQuery, CanBeNull] string referral
            )
        {
            if (!string.IsNullOrEmpty(referral))
            {
                TempData[Referral] = referral;
            }

            if (userAgent != null && userAgent.Contains("linkedin", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.fbImage = ViewBag.imageSrc = "/images/3rdParty/linkedinShare.png";
            }

            //ViewBag.country = country ?? "us";
           
            return View();
        }

        [Route("logout")]
        public async Task<IActionResult> LogOutAsync([FromServices] SignInManager<User> signInManager)
        {
            await signInManager.SignOutAsync().ConfigureAwait(false);
            TempData.Clear();
            return Redirect("/");
        }
    }
}