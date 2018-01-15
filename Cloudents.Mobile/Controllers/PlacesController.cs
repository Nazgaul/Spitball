using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Mobile.Extensions;
using Cloudents.Mobile.Models;
using Microsoft.Azure.Mobile.Server.Config;

namespace Cloudents.Mobile.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// The food and places api controller
    /// </summary>
    [MobileAppController]
    public class PlacesController : ApiController
    {
        private readonly IGooglePlacesSearch _purchaseSearch;
        private readonly IIpToLocation _ipToLocation;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="purchaseSearch"></param>
        /// <param name="ipToLocation"></param>
        public PlacesController(IGooglePlacesSearch purchaseSearch, IIpToLocation ipToLocation)
        {
            _purchaseSearch = purchaseSearch;
            _ipToLocation = ipToLocation;
        }
        /// <summary>
        /// Query to get food and places vertical
        /// </summary>
        /// <param name="purchaseRequest">The model</param>
        /// <param name="token"></param>
        /// <returns>The list of places and token for paging</returns>
        public async Task<HttpResponseMessage> Get([FromUri]PurchaseRequest purchaseRequest,
            CancellationToken token)
        {
            if (purchaseRequest.Location == null)
            {
                var location = Request.GetClientIp();
                var locationResult = await _ipToLocation.GetAsync(IPAddress.Parse(location), token);
                purchaseRequest.Location = locationResult.ConvertToPoint();
            }

            var result = await _purchaseSearch.SearchNearbyAsync(purchaseRequest.Term, purchaseRequest.Filter.GetValueOrDefault(), purchaseRequest.Location, null, token).ConfigureAwait(false);

            var nextPageLink = Url.Link("DefaultApis", new
            {
                controller = "Places"
            }, new { nextPageToken = result.token });

            return Request.CreateResponse(new
            {
                nextPageLink,
                result.data
            });
        }

        /// <summary>
        /// The paging for next place
        /// </summary>
        /// <param name="nextPageToken">the token</param>
        /// <param name="token"></param>
        /// <returns>The list of places and token for paging</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<HttpResponseMessage> Get([FromUri]string nextPageToken,
            CancellationToken token)
        {
            if (nextPageToken == null) throw new ArgumentNullException(nameof(nextPageToken));
            var result = await _purchaseSearch.SearchNearbyAsync(null,
                PlacesRequestFilter.None, null, nextPageToken, token).ConfigureAwait(false);

            var nextPageLink = Url.Link("DefaultApis", new
            {
                controller = "Places"
            }, new { nextPageToken = result.token });

            return Request.CreateResponse(new
            {
                nextPageLink,
                result.data
            });
        }
    }
}
