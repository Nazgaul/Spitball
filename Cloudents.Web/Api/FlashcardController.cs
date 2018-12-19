﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Entities.DocumentDb;
using Cloudents.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    /// <inheritdoc />
    /// <summary>
    /// Flashcard controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    public class FlashcardController : ControllerBase
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


        //TODO we need to fix that


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
