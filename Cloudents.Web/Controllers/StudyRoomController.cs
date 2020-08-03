using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Query;
using Cloudents.Query.Tutor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;

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

        [Route("live/{id:guid}")]
        public async Task<IActionResult> RedirectLive(Guid id,[FromServices] IStatelessSession session)
        {
            var course = await session.Query<BroadCastStudyRoom>().Where(w => w.Id == id)
                .Select(s => new {s.Course.Id, s.Course.Name}).SingleOrDefaultAsync();
            if (course == null)
            {
                return NotFound();
            }

            ///{course.Name}
            return Redirect($"/course/{course.Id}");

        }
    }
}