using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Entities.DocumentDb;
using Cloudents.Core.Interfaces;
using Cloudents.Mobile.Models;
using Microsoft.Azure.Mobile.Server.Config;

namespace Cloudents.Mobile.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Flashcard controller
    /// </summary>
    [MobileAppController]
    public class FlashcardController : ApiController
    {
        private readonly IReadRepositoryAsync<Flashcard, long> _repository;

        /// <inheritdoc />
        /// <summary>
        /// ctor
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
        public async Task<IHttpActionResult> Get(long id, CancellationToken token)
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

        [HttpPost, Route("api/flashcard/like")]
        [Authorize , Obsolete]
        public HttpResponseMessage AddLike(ItemLikeRequest model)
        {
            //var command = new AddFlashcardLikeCommand(User.GetUserId(), model.Id);
            //await _zboxWriteService.AddFlashcardLikeAsync(command).ConfigureAwait(false);

            //if (model.Tags != null && model.Tags.Any())
            //{
            //    var z = new AssignTagsToFlashcardCommand(model.Id, model.Tags, TagType.User);
            //    await _zboxWriteService.AddItemTagAsync(z).ConfigureAwait(false);
            //}

            return Request.CreateResponse(HttpStatusCode.OK, Guid.NewGuid());
        }
        [HttpDelete, Route("api/flashcard/like")]
        [Authorize, Obsolete]
        public HttpResponseMessage DeleteLike(Guid likeId)
        {
            //var command = new DeleteFlashcardLikeCommand(User.GetUserId(), likeId);
            //await _zboxWriteService.DeleteFlashcardLikeAsync(command).ConfigureAwait(false);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
