using System.Linq;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Api
{
    /// <inheritdoc />
    /// <summary>
    /// University api controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController, Authorize]
    public class UniversityController : ControllerBase
    {
        private readonly IUniversitySearch _universityProvider;

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="universityProvider"></param>
        public UniversityController(IUniversitySearch universityProvider)
        {
            _universityProvider = universityProvider;
        }

        /// <summary>
        /// Get list of universities
        /// </summary>
        /// <param name="model">object of query string</param>
        /// <param name="token"></param>
        /// <returns>list of universities</returns>
        [HttpGet]
        public async Task<UniversityResponse> GetAsync([FromQuery] UniversityRequest model, CancellationToken token)
        {
            var countryClaim = User.Claims.FirstOrDefault(f => f.Type == "country");
            var result = await _universityProvider.SearchAsync(model.Term,
                countryClaim?.Value, token).ConfigureAwait(false);
            return new UniversityResponse(result);
        }
    }
}
