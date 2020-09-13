using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Query;
using Cloudents.Query.Tutor;
using Cloudents.Web.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StudyRoomController : Controller
    {
        internal const string StudyRoomRouteName = "StudyRoomRoute";
        private readonly IQueryBus _queryBus;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<User> _signInManager;

        public StudyRoomController(IQueryBus queryBus, IHttpContextAccessor httpContextAccessor, SignInManager<User> signInManager)
        {
            _queryBus = queryBus;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
        }

        [Route("StudyRoom/{id:guid}", Name = StudyRoomRouteName), SignInWithToken]
        public async Task<IActionResult> Index(Guid id,
            [FromQuery] string? dialog,
            [FromQuery] string? type,
            CancellationToken token)
        {
            ViewBag.isRtl = false;
            if (dialog == "payment")
            {
                // this hotfix that happens on client side
                return RedirectToAction("Index");
            }

            if (string.Equals(type, Api.StudyRoomController.TailorEdStudyRoomTypeQueryString,
                StringComparison.OrdinalIgnoreCase) && _signInManager.IsSignedIn(User))
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", new
                {
                    type =  Api.StudyRoomController.TailorEdStudyRoomTypeQueryString
                });
            }

            var result = await _queryBus.QueryAsync(new SeoStudyRoomQuery(id), token);
            if (result == null)
            {
                return NotFound();
            }
            ViewBag.title = $"{result.Name} | {result.TutorName}";
            ViewBag.metaDescription = result.Description;
            ViewBag.ogImage = "https://" + _httpContextAccessor.HttpContext.Request.Host + "/images/3rdParty/fb-share-Spitball-live.png";
            return View("Index");
        }

        [Route("live/{id:guid}")]
        [Route("course/{id:guid}")]
        public async Task<IActionResult> RedirectLive(Guid id, [FromServices] IStatelessSession session)
        {
            var course = await session.Query<BroadCastStudyRoom>().Where(w => w.Id == id)
                .Select(s => new { s.Course.Id, s.Course.Name }).SingleOrDefaultAsync();
            if (course == null)
            {
                return NotFound();
            }

            //{course.Name}
            return Redirect($"/course/{course.Id}");

        }


        //https://www.spitball.co/course/9fe69dc0-f8c6-4ccc-a1aa-ac0c0134485a?t=1596480572534
    }
}