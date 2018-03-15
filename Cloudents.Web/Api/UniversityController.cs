using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/University")]
    public class UniversityController : Controller
    {
        private readonly IUniversitySearch _universityProvider;

        public UniversityController(IUniversitySearch universityProvider)
        {
            _universityProvider = universityProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string term, Location location, CancellationToken token)
        {
            var result = await _universityProvider.SearchAsync(term, location.Point, token).ConfigureAwait(false);
            return Json(result);
        }

        [HttpGet("approximate")]
        public async Task<IActionResult> ByApproximateAsync(GeoPoint point, CancellationToken token)
        {
            var result = await _universityProvider.GetApproximateUniversitiesAsync(point, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}