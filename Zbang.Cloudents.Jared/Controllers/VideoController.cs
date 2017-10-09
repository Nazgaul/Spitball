using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Mobile.Server.Config;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class VideoController : ApiController
    {
        private readonly IVideoSearch m_VideoSearch;

        public VideoController(IVideoSearch videoSearch)
        {
            m_VideoSearch = videoSearch;
        }

        public async Task<HttpResponseMessage> Get(string term, CancellationToken token)
        {
            var result = await m_VideoSearch.SearchAsync(term, token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }
    }
}
