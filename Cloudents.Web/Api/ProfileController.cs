using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        private readonly IQueryBus _queryBus;

        public ProfileController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        // GET
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(long id, CancellationToken token)
        {
            var query = new UserDataByIdQuery(id);
            var retVal = await _queryBus.QueryAsync<ProfileDto>(query, token).ConfigureAwait(false);
            if (retVal == null)
            {
                return NotFound();
            }
            return Ok(retVal);
        }
    }
}