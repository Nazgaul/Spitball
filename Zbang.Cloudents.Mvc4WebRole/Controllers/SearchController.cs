using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [ZboxAuthorize]
    [NoUniversity]
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class SearchController : BaseController
    {
        private readonly IBoxReadSearchProvider2 m_BoxSearchService;
        private readonly IItemReadSearchProvider m_ItemSearchService;
        private readonly IQuizReadSearchProvider2 m_QuizSearchService;
        private readonly IFlashcardReadSearchProvider m_FlashcardSearchService;

        private const int ResultSizeRegularState = 25;

        public SearchController(IBoxReadSearchProvider2 boxSearchService,
            IItemReadSearchProvider itemSearchService,
            IQuizReadSearchProvider2 quizSearchService, IFlashcardReadSearchProvider flashcardSearchService)
        {
            m_BoxSearchService = boxSearchService;
            m_ItemSearchService = itemSearchService;
            m_QuizSearchService = quizSearchService;
            m_FlashcardSearchService = flashcardSearchService;
        }

        [Route("Search")]
        public ActionResult Index(string q)
        {
            if (string.IsNullOrEmpty(q))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View("Empty");
        }

        public ActionResult IndexPartial()
        {
            return PartialView("Index");
        }


        [HttpGet,ActionName("Boxes")]
        public async Task<JsonResult> BoxesAsync(string q, int page, CancellationToken cancellationToken)
        {
            var universityDataId = User.GetUniversityDataId();
            if (!universityDataId.HasValue) return JsonError();
            var query = new SearchBoxesQuery(q, User.GetUserId(), universityDataId.Value, page,
                ResultSizeRegularState);
            return await SearchQueryAsync(q, cancellationToken, query, m_BoxSearchService.SearchBoxAsync).ConfigureAwait(false);
        }

        [HttpGet, ActionName("Flashcards")]
        public async Task<JsonResult> FlashcardsAsync(string q, int page, CancellationToken cancellationToken)
        {
            var universityDataId = User.GetUniversityDataId();
            if (!universityDataId.HasValue) return JsonError();
            var query = new SearchFlashcardQuery(q, User.GetUserId(), universityDataId.Value, page,
                ResultSizeRegularState);
            return await SearchQueryAsync(q, cancellationToken, query, m_FlashcardSearchService.SearchFlashcardAsync).ConfigureAwait(false);
        }

        [HttpGet,ActionName("Items")]
        public async Task<JsonResult> ItemsAsync(string q, int page, CancellationToken cancellationToken)
        {
            var universityDataId = User.GetUniversityDataId();
            if (!universityDataId.HasValue) return JsonError();
            var query = new SearchItemsQuery(q, User.GetUserId(), universityDataId.Value, page,
                ResultSizeRegularState);
            return await SearchQueryAsync(q, cancellationToken, query, m_ItemSearchService.SearchItemAsync).ConfigureAwait(false);
        }

        [HttpGet,ActionName("Quizzes")]
        public async Task<JsonResult> QuizzesAsync(string q, int page, CancellationToken cancellationToken)
        {
            var universityDataId = User.GetUniversityDataId();
            if (!universityDataId.HasValue) return JsonError();
            var query = new SearchQuizesQuery(q, User.GetUserId(), universityDataId.Value, page,
                ResultSizeRegularState);
            return await SearchQueryAsync(q, cancellationToken, query, m_QuizSearchService.SearchQuizAsync).ConfigureAwait(false);
        }

        private async Task<JsonResult> SearchQueryAsync<TD>(string q, CancellationToken cancellationToken,
            SearchQuery query,
            Func<SearchQuery, CancellationToken, Task<TD>> func)
            where TD : class
        {
            try
            {
                if (string.IsNullOrEmpty(q))
                {
                    return JsonError("need term");
                }
                using (var source = CreateCancellationToken(cancellationToken))
                {
                    var retVal = await func(query, source.Token).ConfigureAwait(false);
                    return JsonOk(retVal);
                }
            }
            catch (OperationCanceledException)
            {
                return JsonOk();
            }
        }

        [HttpGet,ActionName("MembersInBox")]
        public async Task<JsonResult> MembersInBoxAsync(string term, long boxId, int page, int sizePerPage = 20)
        {
            if (string.IsNullOrEmpty(term))
            {
                term = "";
            }
            var universityId = User.GetUniversityDataId();
            if (!universityId.HasValue)
                return JsonError("need university");
            var query = new UserInBoxSearchQuery(term, universityId.Value, boxId, page, sizePerPage);
            var retVal = await ZboxReadService.GetUsersInBoxByTermAsync(query).ConfigureAwait(false);
            return JsonOk(retVal);
        }

        [HttpGet,ActionName("ItemInBox")]
        public async Task<JsonResult> ItemInBoxAsync(string term, long boxId, int page, CancellationToken cancellationToken, int sizePerPage = 20)
        {
            if (string.IsNullOrEmpty(term))
            {
                return JsonError();
            }
            try
            {
                var query = new SearchItemInBox(term, boxId, page, sizePerPage);
                using (var source = CreateCancellationToken(cancellationToken))
                {
                    var retVal = await m_ItemSearchService.SearchItemAsync(query, source.Token).ConfigureAwait(false);
                    return JsonOk(retVal);
                }
            }
            catch (OperationCanceledException)
            {
                return JsonOk();
            }
        }
    }
}