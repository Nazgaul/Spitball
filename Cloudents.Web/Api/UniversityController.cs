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

        private readonly IUniversitySearch m_UniversityProvider;

        public UniversityController(IUniversitySearch universityProvider)
        {
            m_UniversityProvider = universityProvider;
        }
        [HttpGet]
        public async Task<IActionResult> Get(string term, GeoPoint location, CancellationToken token)
        {
            var result = await m_UniversityProvider.SearchAsync(term, location, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}