using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        // GET
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(long id, [FromServices] IUserRepository repository, CancellationToken token)
        {
            var retVal = await repository.GetUserProfileAsync(id, token).ConfigureAwait(false);
            if (retVal == null)
            {
                return NotFound();
            }
            return Ok(retVal);
        }
    }
}