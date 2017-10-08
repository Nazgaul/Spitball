using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;

namespace Zbang.Cloudents.Jared.Controllers
{
    public class PlacesController : ApiController
    {
        private readonly IPurchaseSearch m_PurchaseSearch;

        public PlacesController(IPurchaseSearch purchaseSearch)
        {
            m_PurchaseSearch = purchaseSearch;
        }

        public async Task<HttpResponseMessage> Get(string[] term, SearchRequestFilter filter, GeoPoint location, CancellationToken token)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));
            if (location == null) throw new ArgumentNullException(nameof(location));
            var result = await m_PurchaseSearch.SearchAsync(string.Join(" ", term), filter, location, token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }
    }
}
