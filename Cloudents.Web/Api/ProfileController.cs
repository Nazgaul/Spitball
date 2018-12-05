using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IQueryBus _queryBus;

        public ProfileController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        // GET
        [HttpGet("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        
        public async Task<ActionResult<UserProfileDto>> GetAsync(long id, CancellationToken token)
        {
            var query = new UserDataByIdQuery(id);
            var retVal = await _queryBus.QueryAsync<UserProfileDto>(query, token).ConfigureAwait(false);
            if (retVal == null)
            {
                return NotFound();
            }
            return retVal;
        }

        // GET
        [HttpGet("{id}/questions")]
        [ProducesResponseType(200)]

        public async Task<IEnumerable<QuestionFeedDto>> GetQuestionsAsync(long id, int page, CancellationToken token)
        {
            var query = new UserDataPagingByIdQuery(id, page);
            var retVal = await _queryBus.QueryAsync<IEnumerable<QuestionFeedDto>>(query, token).ConfigureAwait(false);
            
            return retVal;
        }

        // GET
        [HttpGet("{id}/answers")]
        [ProducesResponseType(200)]

        public async Task<IEnumerable<QuestionFeedDto>> GetAnswersAsync(long id, int page, CancellationToken token)
        {
            var query = new UserAnswersByIdQuery(id, page);
            var retVal = await _queryBus.QueryAsync<IEnumerable<QuestionFeedDto>>(query, token).ConfigureAwait(false);
            return retVal;
        }

        [HttpGet("{id}/documents")]
        [ProducesResponseType(200)]

        public async Task<IEnumerable<DocumentFeedDto>> GetDocumentsAsync(long id, int page, CancellationToken token)
        {
            var query = new UserDataPagingByIdQuery(id, page);
            var retVal = await _queryBus.QueryAsync<IEnumerable<DocumentFeedDto>>(query, token);

            return retVal.Select(s =>
            {
                 s.Url = Url.DocumentUrl(s.University, s.Course, s.Id, s.Title);
                return s;
            });
        }
    }
}