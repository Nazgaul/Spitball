using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.MobileApi.Extensions;
using Cloudents.MobileApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.MobileApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// University api controller
    /// </summary>
    [Route("api/[controller]")]
    public class UniversityController : Controller
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
        public async Task<IActionResult> Get([FromQuery] UniversityRequest model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetError());
            }
            var result = await _universityProvider.SearchAsync(model.Term, model.Location, token).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
