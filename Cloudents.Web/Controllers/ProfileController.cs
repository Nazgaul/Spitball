using System;
using Cloudents.Core;
using Cloudents.Core.Enum;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.Extensions.Configuration;
using Schema.NET;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ProfileController : Controller
    {
        private readonly IStringLocalizer<ProfileController> _localizer;
        private readonly IQueryBus _queryBus;
        private readonly IConfiguration _configuration;

        public ProfileController(IStringLocalizer<ProfileController> localizer, IQueryBus queryBus, IConfiguration configuration)
        {
            _localizer = localizer;
            _queryBus = queryBus;
            _configuration = configuration;
        }

        [Route("profile/{id:long}")]
        public async Task<IActionResult> OldIndex(long id, CancellationToken token)
        {
            //not really need it in here
            var query = new UserProfileQuery(id, 0);
            var retVal = await _queryBus.QueryAsync(query, token);

            if (retVal == null)
            {
                return NotFound();
            }

            return RedirectToRoutePermanent(SeoTypeString.Tutor, new
            {
                id,
                name = FriendlyUrlHelper.GetFriendlyTitle(retVal.Name)
            });
        }

        // GET: /<controller>/
        [Route("profile/{id:long}/{name}", Name = SeoTypeString.Tutor)]
        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = TimeConst.Hour, NoStore = true), SignInWithToken]
        public async Task<IActionResult> Index(long id, string name, CancellationToken token)
        {
            var query = new UserProfileQuery(id, 0);
            var retVal = await _queryBus.QueryAsync(query, token);
            if (retVal == null)
            {
                return NotFound();
            }

            if (retVal.Tutor is null)
            {
                Response.Headers.Add("X-Robots-Tag", "noindex");
                return View();
            }
            var localizerSuffix = string.Empty;
            if (string.IsNullOrEmpty(retVal.UniversityName))
            {
                localizerSuffix = "NoUniversity";

            }
            ViewBag.title = _localizer[$"Title{localizerSuffix}", retVal.Name, retVal.UniversityName];
            ViewBag.metaDescription = _localizer["Description", retVal.Description];
            ViewBag.fbImage = retVal.Image;

            //var jsonLd = new ProfilePage()
            //{
            //    SourceOrganization = new Organization
            //    {
            //        Logo = new Uri($"{_configuration["site"].TrimEnd('/')}/images/favicons/android-icon-192x192.png"),
            //        Url = Request.GetUri()
            //    },
            //    About = new Person()
            //    {
            //        Name = retVal.Name,
            //        GivenName = retVal.FirstName,
            //        FamilyName = retVal.LastName,
            //        Image = new Uri($"{_configuration["site"].TrimEnd('/')}{retVal.Image}"), // TODO should be fixed
            //        Description = retVal.Description,
            //    },
            //    Url = Request.GetUri()
            //};
            //ViewBag.jsonLd = jsonLd;
            return View();
        }
    }
}
