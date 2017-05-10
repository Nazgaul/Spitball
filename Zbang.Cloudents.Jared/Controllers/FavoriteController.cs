using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries.Jared;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class FavoriteController : ApiController
    {
        private readonly IZboxReadService m_ZboxReadService;
        private readonly IDocumentDbReadService m_DocumentDbReadService;

        public FavoriteController(IZboxReadService zboxReadService, IDocumentDbReadService documentDbReadService)
        {
            m_ZboxReadService = zboxReadService;
            m_DocumentDbReadService = documentDbReadService;
        }

        // GET api/Favorite

        public async Task<HttpResponseMessage> Get(IEnumerable<long> documentIds, IEnumerable<long> quizIds, IEnumerable<long> flashcardIds)
        {
            var query = new JaredFavoritesQuery(documentIds, quizIds);
            var t2 = m_DocumentDbReadService.FavoriteFlashcardsAsync(flashcardIds);
            var t1 = m_ZboxReadService.JaredFavoritesAsync(query);
            await Task.WhenAll(t1, t2).ConfigureAwait(false);
            var retVal = t1.Result;
            return Request.CreateResponse(new
            {
                document = retVal.Item1,
                quiz = retVal.Item2,
                flashcard = t2.Result
            });
        }
    }
}
