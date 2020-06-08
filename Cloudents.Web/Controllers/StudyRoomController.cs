using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Query;
using Cloudents.Query.Tutor;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StudyRoomController : Controller
    {
        private readonly IQueryBus _queryBus;

        public StudyRoomController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
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
            return View("Index");
        }
    }
}