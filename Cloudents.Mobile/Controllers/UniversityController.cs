using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Interfaces;
using Cloudents.Mobile.Extensions;
using Cloudents.Mobile.Models;
using Microsoft.Azure.Mobile.Server.Config;

namespace Cloudents.Mobile.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// University api controller
    /// </summary>
    [MobileAppController]
    public class UniversityController : ApiController
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
        public async Task<IHttpActionResult> Get([FromUri] UniversityRequest model, CancellationToken token)
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
