using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using Microsoft.AspNetCore.Mvc;
using Cloudents.Ico.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Cloudents.Ico.Controllers
{
    public class HomeController : Controller
    {
        private readonly Lazy<IServiceBusProvider> _serviceBus;

        public HomeController(Lazy<IServiceBusProvider> serviceBus)
        {
            _serviceBus = serviceBus;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("contact")]
        public async Task<IActionResult> ContactUs(ContactUs model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _serviceBus.Value.InsertMessageAsync(new ContactUsEmail(model.Name, model.Email, model.Text), token);
            return Ok();
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe(Subscribe model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _serviceBus.Value.InsertMessageAsync(new ContactUsEmail(model.Email), token);
            return Ok();
        }

        public IActionResult SetLanguage(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect("/");
        }

        public async Task<RedirectResult> DownloadWhitePaper([FromServices] IBlobProvider<IcoContainer> blobProvider,CancellationToken token)
        {
            var currentCulture = CultureInfo.CurrentCulture;

            var blobName = $"Spitball-WP.{currentCulture.Name}.pdf";
            var exists = await blobProvider.ExistsAsync(blobName, token);
            if (exists)
            {
                var url = blobProvider.GetBlobUrl(blobName).AbsoluteUri;
                return Redirect(url);
            }
            return Redirect("https://zboxstorage.blob.core.windows.net/zboxhelp/ico/Spitball-WP.pdf");
        }
    }
}
