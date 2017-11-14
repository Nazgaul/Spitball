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
        public async Task<IActionResult> Get(string term, GeoPoint location, CancellationToken token)
        {
            var result = await _universityProvider.SearchAsync(term, location, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}