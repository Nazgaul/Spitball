using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.Jared.Extensions;
using Zbang.Cloudents.Jared.Models;

namespace Zbang.Cloudents.Jared.Controllers
{
    /// <summary>
    /// The food and places api controller
    /// </summary>
    [MobileAppController]
    public class PlacesController : ApiController
    {
        private readonly IPlacesSearch _purchaseSearch;
        private readonly IIpToLocation _ipToLocation;

        public PlacesController(IPlacesSearch purchaseSearch, IIpToLocation ipToLocation)
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
            if (purchaseRequest.Term == null) throw new ArgumentNullException(nameof(purchaseRequest.Term));
            if (purchaseRequest.Location == null)
            {
                var location = Request.GetClientIp();
                var locationResult = await _ipToLocation.GetAsync(IPAddress.Parse(location), token);
                purchaseRequest.Location = locationResult.ConvertToPoint();
            }

            var result = await _purchaseSearch.SearchNearbyAsync(string.Join(" ", purchaseRequest.Term), purchaseRequest.Filter.GetValueOrDefault(), purchaseRequest.Location, null, token).ConfigureAwait(false);

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
            var result = await _purchaseSearch.SearchNearbyAsync(string.Empty,
                default, default, nextPageToken, token).ConfigureAwait(false);

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
