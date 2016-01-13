﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Query;
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

        private const int ResultSizeRegularState = 25;

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
        public async Task<JsonResult> Boxes(string q, int page, CancellationToken cancellationToken)
        {
            var universityDataId = User.GetUniversityData();
            if (!universityDataId.HasValue) return JsonError();
            var query = new SearchBoxesQuery(q, User.GetUserId(), universityDataId.Value, page,
                ResultSizeRegularState);
            return await SearchQueryAsync(q, cancellationToken, query, m_BoxSearchService.SearchBox);
        }
        [HttpGet]
        public async Task<JsonResult> Items(string q, int page, CancellationToken cancellationToken)
        {
            var universityDataId = User.GetUniversityData();
            if (!universityDataId.HasValue) return JsonError();
            var query = new SearchItemsQuery(q, User.GetUserId(), universityDataId.Value, page,
                ResultSizeRegularState);
            return await SearchQueryAsync(q, cancellationToken, query, m_ItemSearchService.SearchItemAsync);
        }
        [HttpGet]
        public async Task<JsonResult> Quizzes(string q, int page, CancellationToken cancellationToken)
        {
            var universityDataId = User.GetUniversityData();
            if (!universityDataId.HasValue) return JsonError();
            var query = new SearchQuizesQuery(q, User.GetUserId(), universityDataId.Value, page,
                ResultSizeRegularState);
            return await SearchQueryAsync(q, cancellationToken, query,  m_QuizSearchService.SearchQuiz);
        }

        private async Task<JsonResult> SearchQueryAsync<TD>(string q, CancellationToken cancellationToken, 
            SearchQuery query,
            Func<SearchQuery, CancellationToken, Task<TD>> func)
            where TD : class
        {
            var source = CreateCancellationToken(cancellationToken);
            try
            {
                if (string.IsNullOrEmpty(q))
                {

                    var result = await m_WithCache.QueryAsync(func, query, source.Token);
                    return JsonOk(result);
                }
                var retVal = await func(query, source.Token);
                return JsonOk(retVal);
            }
            catch (OperationCanceledException)
            {
                TraceLog.WriteInfo("search - abort");
                return JsonOk();
            }
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
            var source = CreateCancellationToken(cancellationToken);
            if (string.IsNullOrEmpty(term))
            {
                return JsonError();
            }
            try
            {
                var query = new SearchItemInBox(term, boxId, page, sizePerPage);
                var retVal = await m_ItemSearchService.SearchItemAsync(query, source.Token);
                return JsonOk(retVal);
            }
            catch (OperationCanceledException)
            {
                TraceLog.WriteInfo("items in box - abort");
                return JsonOk();
            }
        }






    }
}