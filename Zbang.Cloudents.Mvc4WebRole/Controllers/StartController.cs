using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IWitAi m_WitAi;

        public StartController(IDetectLanguage detectLanguage, ILuisAi luisAi, IWitAi witAi)
        {
            m_DetectLanguage = detectLanguage;
            m_LuisAi = luisAi;
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
        public async Task<JsonResult> IntentAsync(string term)
        {
            

            if (string.IsNullOrEmpty(term))
            {
                return JsonError();
            }
            var lang = m_DetectLanguage.DoWork(term);
            // if (lang == Zbox.Infrastructure.Culture.Language.EnglishUs)
            // {
            var tLuisData = m_LuisAi.GetUserIntentAsync(term);
            var tWitData = m_WitAi.GetUserIntentAsync(term);

            await Task.WhenAll(tLuisData, tWitData);
            // }
            return JsonOk(new
            {
                lang,
                luis = tLuisData.Result,
                wit = tWitData.Result,
                luisIntent = tLuisData.Result?.GetType().Name,
                witIntent = tWitData.Result?.GetType().Name
            });
        }
    }
}