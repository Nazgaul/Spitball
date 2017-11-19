using System;
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
    public class PlacesController : ApiController
    {
        private readonly IPlacesSearch _purchaseSearch;

        public PlacesController(IPlacesSearch purchaseSearch)
        {
            _purchaseSearch = purchaseSearch;
        }

        public async Task<HttpResponseMessage> Get([FromUri]string[] term,
            PlacesRequestFilter filter,
            GeoPoint location,
            CancellationToken token)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));
            if (location == null) throw new ArgumentNullException(nameof(location));
            var result = await _purchaseSearch.SearchNearbyAsync(string.Join(" ", term), filter, location, null, token).ConfigureAwait(false);
            return Request.CreateResponse(new
            {
                result.token,result.data
            });
        }

        public async Task<HttpResponseMessage> Get([FromUri]string nextPageToken,
            CancellationToken token)
        {
            if (nextPageToken == null) throw new ArgumentNullException(nameof(nextPageToken));
            var result = await _purchaseSearch.SearchNearbyAsync(string.Empty,
                default, default, nextPageToken, token).ConfigureAwait(false);
            return Request.CreateResponse(new
            {
                result.token,
                result.data
            });
        }


    }
}
