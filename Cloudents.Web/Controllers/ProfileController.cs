﻿using System.Linq;
using Cloudents.Core;
using Cloudents.Core.Enum;
using Cloudents.Query;
using Cloudents.Query.Users;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ProfileController : Controller
    {
        private readonly IStringLocalizer<ProfileController> _localizer;
        private readonly IQueryBus _queryBus;
        private readonly IUrlBuilder _urlBuilder;

        public ProfileController(IStringLocalizer<ProfileController> localizer, IQueryBus queryBus, IUrlBuilder urlBuilder)
        {
            _localizer = localizer;
            _queryBus = queryBus;
            _urlBuilder = urlBuilder;
        }


        [Route("profile/{id:long}")]
        [Route("p/{id:long}")]
        public async Task<IActionResult> OldIndexAsync(long id, CancellationToken token)
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
        public async Task<IActionResult> IndexAsync(long id, string name, CancellationToken token)
        {
            var query = new UserProfileQuery(id, 0);
            var retVal = await _queryBus.QueryAsync(query, token);
            if (retVal is null)
            {
                return NotFound();
            }

            if (retVal.Tutor is null)
            {
                Response.Headers.Add("X-Robots-Tag", "noindex");
                return View("Index");
            }

            ViewBag.title = _localizer["TitleNoUniversity", retVal.Name];
            ViewBag.metaDescription = _localizer["Description", retVal.Tutor.Description];
            if (retVal.Image != null)
            {
               // Country country = Country.FromCountry(retVal.Tutor.TutorCountry);

                ViewBag.ogImage = _urlBuilder.BuildUserImageProfileShareEndpoint(retVal.Id, new
                {
                    width = 1200,
                    height = 630,
                    rtl = retVal.Tutor.TutorCountry.MainLanguage.Info.TextInfo.IsRightToLeft.ToString()
                });
                ViewBag.ogImageWidth = 1200;
                ViewBag.ogImageHeight = 630;
                ViewBag.ogTitle = retVal.Name;
                if (retVal.Tutor.Subjects?.Any() == true)
                {
                    ViewBag.ogDescription =
                        _localizer.WithCulture(retVal.Tutor.TutorCountry.MainLanguage)["OgDescription",
                            string.Join(", ", retVal.Tutor.Subjects)];
                }

            }

            return View("Index");
        }
    }
}
