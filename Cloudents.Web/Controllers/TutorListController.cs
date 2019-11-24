using Cloudents.Core.Enum;
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

        [Route("tutor-list/{term?}", Name = SeoTypeString.TutorList)]
        public IActionResult Index(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                ViewBag.Title = _tutorListLocalizer["Title"];
                ViewBag.metaDescription = _tutorListLocalizer["Description"];
            }
            else
            {
                ViewBag.Title = _tutorListLocalizer["Title with Term", term];
                ViewBag.metaDescription = _tutorListLocalizer["Description with Term", term];
            }

            return View();
        }
    }
}