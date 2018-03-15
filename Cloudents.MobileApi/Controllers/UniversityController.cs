using System.Threading;
using System.Threading.Tasks;
using Cloudents.Api.Filters;
using Cloudents.Api.Models;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Api.Controllers
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
        [ValidateModel]
        public async Task<IActionResult> GetAsync([FromQuery] UniversityRequest model, CancellationToken token)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            var result = await _universityProvider.SearchAsync(model.Term, model.Location, token).ConfigureAwait(false);
            return Ok(new
            {
                universities = result
            });
        }

        [HttpGet("approximate")]
        public async Task<IActionResult> ByApproximateAsync([FromQuery]GeoPoint point, CancellationToken token)
        {
            if (point == null)
            {
                return BadRequest();
            }
            var result = await _universityProvider.GetApproximateUniversitiesAsync(point, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}
