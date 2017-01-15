using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Ai;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    public class StartController : BaseController
    {
        private readonly IWitAi m_WitAi;

        public StartController(IWitAi witAi)
        {
            m_WitAi = witAi;
        }

        [AllowAnonymous]
        [DonutOutputCache(CacheProfile = "FullPage")]
        [Route("Start", Name = "start")]
        // GET: Start
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("homePage");
            }
            return View("Empty");
        }

        [HttpGet]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult IndexPartial()
        {
            return PartialView("Index");
        }

        [HttpGet, ActionName("Intent")]
        public async Task<JsonResult> IntentAsync(string term, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(term))
            {
                return JsonError();
            }
            using (var token = base.CreateCancellationToken(cancellationToken))
            {
                var data = await m_WitAi.GetUserIntentAsync(term, token.Token);
                return JsonOk(new
                {
                    data,
                    sentence = data?.ToString()
                });
            }
        }
    }
}