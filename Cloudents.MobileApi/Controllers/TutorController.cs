using System.Threading;
using System.Threading.Tasks;
using Cloudents.Api.Extensions;
using Cloudents.Api.Models;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Api.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Tutor api controller
    /// </summary>
    [Route("api/[controller]", Name = "Tutor")]
    public class TutorController : Controller
    {
        private readonly ITutorSearch _tutorSearch;

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tutorSearch"></param>
        public TutorController(ITutorSearch tutorSearch)
        {
            _tutorSearch = tutorSearch;
        }

        /// <summary>
        /// Get Tutors
        /// </summary>
        /// <param name="model">The model to parse</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery]TutorRequest model, CancellationToken token)
        {
            var t =  Request.GetCapabilities();
            var isMobile = t?.IsMobileDevice ?? true;
            var result = (await _tutorSearch.SearchAsync(model.Term,
                model.Filter,
                model.Sort.GetValueOrDefault(TutorRequestSort.Relevance),
                model.Location,
                model.Page.GetValueOrDefault(), isMobile, token).ConfigureAwait(false)).ToListIgnoreNull();

            string nextPageLink = null;
            if (result.Count > 0)
            {
                nextPageLink = Url.NextPageLink("Tutor", null, model);
            }

            return Ok(new
            {
                result,
                nextPageLink
            });
        }
    }
}
