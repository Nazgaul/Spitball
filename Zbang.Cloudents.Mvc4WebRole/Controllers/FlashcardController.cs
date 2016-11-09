using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Domain.Commands;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxAuthorize]
    public class FlashcardController : BaseController
    {
        // GET: Flashcard
        public ActionResult Index()
        {
            return View();
        }

        [ZboxAuthorize]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult CreatePartial()
        {
            return PartialView();
        }

        [HttpPost, ZboxAuthorize, ActionName("Create")]
        public async Task<JsonResult> CreateAsync(Flashcard model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var command = new AddFlashcardCommand(model);
            await ZboxWriteService.AddFlashcardAsync(command);
            return JsonOk(model);
        }
    }
}