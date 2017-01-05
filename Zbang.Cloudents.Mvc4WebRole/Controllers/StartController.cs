using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IDetectLanguage m_DetectLanguage;
        private readonly ILuisAi m_LuisAi;

        public StartController(IDetectLanguage detectLanguage, ILuisAi luisAi)
        {
            m_DetectLanguage = detectLanguage;
            m_LuisAi = luisAi;
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
        public async Task<JsonResult> IntentAsync(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return JsonError();
            }
            var lang = m_DetectLanguage.DoWork(term);
            // if (lang == Zbox.Infrastructure.Culture.Language.EnglishUs)
            // {
            var data = await m_LuisAi.GetUserIntentAsync(term);
            // }
            return JsonOk(new
            {
                lang,
                data,
                intent = data?.GetType().Name
            });
        }
    }
}