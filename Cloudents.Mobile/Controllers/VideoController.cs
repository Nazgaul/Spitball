using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Mobile.Server.Config;

namespace Cloudents.Mobile.Controllers
{
    [Obsolete]
    [MobileAppController]
    public class VideoController : ApiController
    {
        private readonly IVideoSearch _videoSearch;

        public VideoController(IVideoSearch videoSearch)
        {
            _videoSearch = videoSearch;
        }

        public async Task<HttpResponseMessage> Get(string term, CancellationToken token)
        {
            var result = await _videoSearch.SearchAsync(new[] { term }, token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }
    }
}
