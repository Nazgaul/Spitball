using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Video")]
    public class VideoController : Controller
    {
        private readonly IVideoSearch _videoSearch;

        public VideoController(IVideoSearch videoSearch)
        {
            _videoSearch = videoSearch;
        }

        public async Task<IActionResult> Get(string term, CancellationToken token)
        {
            var result = await _videoSearch.SearchAsync(term, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}