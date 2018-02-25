using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Tutor")]
    public class TutorController : Controller
    {
        private readonly ITutorSearch _tutorSearch;

        public TutorController(ITutorSearch tutorSearch)
        {
            _tutorSearch = tutorSearch;
        }

        public async Task<IActionResult> Get(string[] term,
            TutorRequestFilter[] filter,
            TutorRequestSort? sort,
            Location location, int page, CancellationToken token)
        {
            var isMobile = false;
            var userAgent = Request.Headers["User-Agent"].ToString();
            if (!string.IsNullOrEmpty(userAgent))
            {
                var capabilities = new HttpBrowserCapabilities
                {
                    Capabilities = new Hashtable { { string.Empty, userAgent } }
                };
                isMobile = capabilities.IsMobileDevice;
            }
            var result = await _tutorSearch.SearchAsync(term, filter, sort.GetValueOrDefault(TutorRequestSort.Price), location.Point, page, isMobile, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}