using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.DocumentDb;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Api.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Flashcard controller
    /// </summary>
    [Route("api/[controller]")]
    public class FlashcardController : Controller
    {
        private readonly IReadRepositoryAsync<Flashcard, long> _repository;

        /// <inheritdoc />
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="repository">repository</param>
        public FlashcardController(IReadRepositoryAsync<Flashcard, long> repository)
        {
            _repository = repository;
        }


        /// <summary>
        /// Get flashcard data
        /// </summary>
        /// <param name="id">flashcard id</param>
        /// <param name="token">token</param>
        /// <returns>flashcard data</returns>
        /// <exception cref="ArgumentException">the flashcard is deleted or not publish</exception>
        [HttpGet]
        public async Task<IActionResult> GetAsync(long id, CancellationToken token)
        {
            var result = await _repository.GetAsync(id, token).ConfigureAwait(false);
            if (!result.Publish)
            {
                throw new ArgumentException("Flashcard is not published");
            }
            if (result.IsDeleted)
            {
                throw new ArgumentException("Flashcard is deleted");
            }
            return Ok(new
            {
                result.Name,
                result.Cards
            });
        }
    }
}
