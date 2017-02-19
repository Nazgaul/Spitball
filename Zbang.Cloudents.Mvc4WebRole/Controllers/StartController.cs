using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Features.Indexed;
using DevTrends.MvcDonutCaching;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Ai;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    public class StartController : BaseController
    {
        //private readonly IWitAi m_WitAi;
        //private readonly ILifetimeScope m_Container;
        private readonly IIndex<SearchType, ISearchReadProvider> m_States;
        private readonly IDetectLanguage m_LanguageDetect;

        public StartController(/*IWitAi witAi,*/ IIndex<SearchType, ISearchReadProvider> states, IDetectLanguage languageDetect)
        {
            //m_WitAi = witAi;
            m_States = states;
            m_LanguageDetect = languageDetect;
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
        public JsonResult IntentAsync(string term, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(term))
            {
                return JsonError();
            }
            //using (var token = CreateCancellationToken(cancellationToken))
            //{
                //var data = await m_WitAi.GetUserIntentAsync(term, token.Token);
                //var knownData = data as KnownIntent;
                //if (knownData != null)
                //{
                //    if (knownData.SearchType == SearchType.None)
                //    {
                //        return JsonOk("I need more data");
                //    }
                //    var provider = m_States[knownData.SearchType];
                //    var lang = m_LanguageDetect.DoWork(term);
                //    var result = await provider.SearchAsync(knownData, new SearchJared(lang), token.Token);
                //    return JsonOk(result);
                //}
                //else
                //{
                    return JsonOk("I dont know what you talking about");
                //}

            //}
        }
    }
}