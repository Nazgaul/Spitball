﻿using Cloudents.Core.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TutorListController : Controller
    {
        private readonly IStringLocalizer<TutorListController> _tutorListLocalizer;

        public TutorListController(IStringLocalizer<TutorListController> tutorListLocalizer)
        {
            _tutorListLocalizer = tutorListLocalizer;
        }

        [Route("learn/{term?}", Name = SeoTypeString.TutorList)]
        public IActionResult Index([FromRoute]string term,
            [FromQuery(Name = "term")]string oldTerm)
        {

            if (string.IsNullOrEmpty(term))
            {
                if (!string.IsNullOrEmpty(oldTerm))
                {
                    return RedirectToRoute(SeoTypeString.TutorList, new {term = oldTerm});
                }
                ViewBag.Title = _tutorListLocalizer["Title"];
                ViewBag.metaDescription = _tutorListLocalizer["Description"];
                ViewBag.ogTitle = _tutorListLocalizer["Title"];
                ViewBag.ogDescription = _tutorListLocalizer["Description"];
            }
            else
            {
                ViewBag.Title = _tutorListLocalizer["Title with Term", term];
                ViewBag.metaDescription = _tutorListLocalizer["Description with Term", term];
                ViewBag.ogTitle = _tutorListLocalizer["Title with Term", term];
                ViewBag.ogDescription = _tutorListLocalizer["Description with Term", term];
            }

            return View();
        }
    }
}