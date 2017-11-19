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
        private readonly ITutorSearch _tutorSearch;

        public TutorController(ITutorSearch tutorSearch)
        {
            _tutorSearch = tutorSearch;
        }

        public async Task<HttpResponseMessage> Get(string term, SearchRequestFilter filter,
            SearchRequestSort? sort, [FromUri] GeoPoint location, int page, CancellationToken token)
        {

            var result = await _tutorSearch.SearchAsync(term, filter, sort.GetValueOrDefault(SearchRequestSort.Price), location, page, token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }
    }
}
