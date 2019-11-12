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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ProfileController : Controller
    {
        private readonly IStringLocalizer<ProfileController> _localizer;
        private readonly IQueryBus _queryBus;

        public ProfileController(IStringLocalizer<ProfileController> localizer, IQueryBus queryBus)
        {
            _localizer = localizer;
            _queryBus = queryBus;
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
            var query = new UserProfileQuery(id,0);
            var retVal = await _queryBus.QueryAsync(query, token);
            if (retVal == null)
            {
                return NotFound();
            }

            if (retVal.Tutor == null)
            {
                Response.Headers.Add("X-Robots-Tag", "noindex");
            }
            var localizerSuffix = string.Empty;
            if (string.IsNullOrEmpty(retVal.UniversityName))
            {
                localizerSuffix = "NoUniversity";

            }
            ViewBag.title = _localizer[$"Title{localizerSuffix}", retVal.Name, retVal.UniversityName];
            ViewBag.metaDescription = _localizer["Description", retVal.Description];
            return View();
        }
    }
}
