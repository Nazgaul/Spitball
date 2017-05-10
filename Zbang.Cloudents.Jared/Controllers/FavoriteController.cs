using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Zbox.Domain;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.JaredDtos;
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
        //[Authorize]
        //one digit params to short query string
        public async Task<HttpResponseMessage> Get([FromUri]IEnumerable<long> d,
            [FromUri]IEnumerable<long> q,
            [FromUri]IEnumerable<long> f, [FromUri] IEnumerable<Guid> c)
        {


            var t2 = Task.FromResult<IEnumerable<Flashcard>>(new Flashcard[0]);
            var flashcardIdList = f as IList<long> ?? f.ToList();
            if (flashcardIdList.Any())
            {
                t2 = m_DocumentDbReadService.FavoriteFlashcardsAsync(flashcardIdList);
            }
            var query = new JaredFavoritesQuery(d, q, flashcardIdList, c);
            var t1 = m_ZboxReadService.JaredFavoritesAsync(query);
            await Task.WhenAll(t1, t2).ConfigureAwait(false);
            var retVal = t1.Result;

            return Request.CreateResponse(new
            {
                document = retVal.Documents,
                quiz = retVal.Quizzes,
                comment = retVal.Comments,
                flashcard = t2.Result.Select(s =>
                {
                    var card = s.Cards.FirstOrDefault();
                    return new JaredFavoriteFlashcardDto
                    {
                        Id = s.Id,
                        Date = s.DateTime,
                        CardFront = card?.Front?.Image ?? card?.Front?.Text,
                        CardBack = card?.Cover?.Image ?? card?.Cover?.Text,
                        CardCount = s.Cards.Count(),
                        Likes = retVal.Flashcards.FirstOrDefault(f1 => f1.Id == s.Id)?.Likes ?? 0,
                        Views = retVal.Flashcards.FirstOrDefault(f1 => f1.Id == s.Id)?.Views ?? 0,
                        Name = s.Name
                    };
                })
            });
        }
    }
}
