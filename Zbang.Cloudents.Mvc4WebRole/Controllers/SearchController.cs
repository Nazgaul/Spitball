﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.SiteExtension;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Search;
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
        private readonly IBoxReadSearchProvider2 m_BoxSearchService;
        private readonly IItemReadSearchProvider2 m_ItemSearchService;
        private readonly IQuizReadSearchProvider2 m_QuizSearchService;
        private readonly IWithCache m_WithCache;

        public SearchController(IBoxReadSearchProvider2 boxSearchService, IItemReadSearchProvider2 itemSearchService, IQuizReadSearchProvider2 quizSearchService, IWithCache withCache)
        {
            m_BoxSearchService = boxSearchService;
            m_ItemSearchService = itemSearchService;
            m_QuizSearchService = quizSearchService;
            m_WithCache = withCache;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Dashboard");
        }

        public ActionResult IndexPartial()
        {
            return PartialView("Index");
        }



        [HttpGet]
        public async Task<ActionResult> Data(string q, int page, CancellationToken cancellationToken)
        {
            CancellationToken disconnectedToken = Response.ClientDisconnectedToken;
            var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, disconnectedToken);
            var emptyState = false;
            try
            {
                var numberOfResult = 15;
                if (string.IsNullOrEmpty(q))
                {
                    numberOfResult = 5;
                    emptyState = true;
                }

                var universityDataId = User.GetUniversityData();
                if (!universityDataId.HasValue) return null;

                var query = new SearchQuery(q, User.GetUserId(),
                   universityDataId.Value, page, numberOfResult);
                if (emptyState)
                {
                    var result = await m_WithCache.QueryAsync(PerformQuery, query, source.Token);
                    return JsonOk(result);
                }

                var retVal = await PerformQuery(query, source.Token);
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

        private async Task<SearchDto> PerformQuery(SearchQuery query, CancellationToken token)
        {
            var t1 = m_BoxSearchService.SearchBox(query, token);
            var t2 = m_ItemSearchService.SearchItemAsync(query, token);
            var t3 = m_QuizSearchService.SearchQuiz(query, token);


            await Task.WhenAll(t1, t2, t3);
            var retVal = new SearchDto
            {
                Boxes = t1.Result,
                Items = t2.Result,
                Quizzes = t3.Result
            };
            return retVal;
        }


        [HttpGet]
        public async Task<JsonResult> Members(string term, long boxId, int page, int sizePerPage = 20)
        {
            if (string.IsNullOrEmpty(term))
            {
                term = "";
            }
            long? universityId = User.GetUniversityData();
            if (!universityId.HasValue)
                return JsonError("need university");
            var query = new UserSearchQuery(term, universityId.Value, boxId, page, sizePerPage);
            var retVal = await ZboxReadService.GetUsersByTermAsync(query);

            return JsonOk(retVal);

        }


        [HttpGet]
        public async Task<JsonResult> ItemInBox(string term, long boxId, int page, CancellationToken cancellationToken, int sizePerPage = 20)
        {
            CancellationToken disconnectedToken = Response.ClientDisconnectedToken;
            var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, disconnectedToken);
            if (string.IsNullOrEmpty(term))
            {
                return JsonError();
            }
            var query = new SearchItemInBox(term, boxId, page, sizePerPage);
            var retVal = await m_ItemSearchService.SearchItemAsync(query, source.Token);
            return JsonOk(retVal);
        }






    }
}