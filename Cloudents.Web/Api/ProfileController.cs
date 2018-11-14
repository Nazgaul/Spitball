using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
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
        
        public async Task<ActionResult<ProfileDto>> GetAsync(long id, CancellationToken token)
        {
            var query = new UserDataByIdQuery(id);
            var retVal = await _queryBus.QueryAsync<ProfileDto>(query, token).ConfigureAwait(false);
            if (retVal == null)
            {
                return NotFound();
            }
            return retVal;
        }

       
    }
}