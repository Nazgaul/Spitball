using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Microsoft.Azure.Mobile.Server.Config;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class TutorController : ApiController
    {
        private readonly ITutorSearch m_TutorSearch;

        public TutorController(ITutorSearch tutorSearch)
        {
            m_TutorSearch = tutorSearch;
        }

        public async Task<HttpResponseMessage> Get(string term, SearchRequestFilter filter, SearchRequestSort sort, [FromUri] GeoPoint location, CancellationToken token)
        {
            var result = await m_TutorSearch.SearchAsync(term, filter, sort, location, token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }
    }
}
