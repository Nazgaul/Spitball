using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Query;
using Cloudents.Query.Tutor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StudyRoomController : Controller
    {
        private readonly IQueryBus _queryBus;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StudyRoomController(IQueryBus queryBus, IHttpContextAccessor httpContextAccessor)
        {
            _queryBus = queryBus;
            _httpContextAccessor = httpContextAccessor;
        }

        [Route("StudyRoom/{id:guid}")]
        public async Task<IActionResult> Index(Guid id, [FromQuery] string? dialog, CancellationToken token)
        {
            ViewBag.isRtl = false;
            if (dialog == "payment")
            {
                // this hotfix that happens on client side
                return RedirectToAction("Index");
            }

            var result = await _queryBus.QueryAsync(new SeoStudyRoomQuery(id), token);
            if (result == null)
            {
                return NotFound();
            }
            ViewBag.title =$"{result.Name} | {result.TutorName}";
            ViewBag.metaDescription = result.Description;
            ViewBag.ogImage = "https://" + _httpContextAccessor.HttpContext.Request.Host + "/images/3rdParty/fb-share-Spitball-live.png";
            return View("Index");
        }
    }
}