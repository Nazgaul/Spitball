using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.DocumentDb;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Flashcard")]
    public class FlashcardController : Controller
    {
        private readonly IReadRepositoryAsync<Flashcard, long> _repository;

        public FlashcardController(IReadRepositoryAsync<Flashcard, long> repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Get(long id, CancellationToken token)
        {
            var result = await _repository.GetAsync(id, token).ConfigureAwait(false);
            return Json(new
            {
                result.Name,
                result.Cards
            });
        }
    }
}