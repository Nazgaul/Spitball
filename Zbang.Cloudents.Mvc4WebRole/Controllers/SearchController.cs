using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.SiteExtension;
using Zbang.Zbox.Infrastructure.Azure.Search;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.Search;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [ZboxAuthorize]
    [NoUniversity]
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class SearchController : BaseController
    {
        private readonly IBoxReadSearchProvider m_BoxSearchService;
        private readonly IItemReadSearchProvider m_ItemSearchService;
        private readonly IQuizReadSearchProvider m_QuizSearchService;

        public SearchController(IBoxReadSearchProvider boxSearchService, IItemReadSearchProvider itemSearchService, IQuizReadSearchProvider quizSearchService)
        {
            m_BoxSearchService = boxSearchService;
            m_ItemSearchService = itemSearchService;
            m_QuizSearchService = quizSearchService;
        }





        [HttpGet]
        public async Task<ActionResult> Data(string q, int page)
        {
            try
            {
                var numberOfResult = 15;
                if (string.IsNullOrEmpty(q))
                {
                    numberOfResult = 5;
                }
                //if (string.IsNullOrWhiteSpace(q))
                //{
                //    return JsonOk();
                //}
                var universityDataId = User.GetUniversityData();
                if (!universityDataId.HasValue) return null;

                var query = new SearchQuery(q, User.GetUserId(),
                    universityDataId.Value, page, numberOfResult);
                var t1 = m_BoxSearchService.SearchBox(query, Response.ClientDisconnectedToken);
                var t2 = m_ItemSearchService.SearchItem(query, Response.ClientDisconnectedToken);
                var t3 = m_QuizSearchService.SearchQuiz(query, Response.ClientDisconnectedToken);


                await Task.WhenAll(t1, t2, t3);
                var retVal = new SearchDto
                {
                    Boxes = t1.Result,
                    Items = t2.Result,
                    Quizzes = t3.Result
                };
                return JsonOk(retVal);
            }
            catch (OperationCanceledException)
            {
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On Seach/Data q: " + q + " page: " + page + "userid: " + User.GetUserId(), ex);
                return JsonError();
            }
        }






    }
}