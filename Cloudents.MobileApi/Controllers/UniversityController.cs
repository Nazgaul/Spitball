using System.Threading;
using System.Threading.Tasks;
using Cloudents.Api.Filters;
using Cloudents.Api.Models;
using Cloudents.Core.Interfaces;
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
            var result = await _universityProvider.SearchAsync(model.Term,
                model.Location.ToGeoPoint(), token).ConfigureAwait(false);
            return Json(result);
        }
    }
}
