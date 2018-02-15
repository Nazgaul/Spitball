using System;
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
        private readonly IPlacesSearch _purchaseSearch;

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="purchaseSearch"></param>
        public PlacesController(IPlacesSearch purchaseSearch)
        {
            _purchaseSearch = purchaseSearch;
        }
        /// <summary>
        /// Query to get food and places vertical
        /// </summary>
        /// <param name="purchaseRequest">The model</param>
        /// <param name="token"></param>
        /// <returns>The list of places and token for paging</returns>
        public async Task<IHttpActionResult> Get([FromUri]PurchaseRequest purchaseRequest,
            CancellationToken token)
        {
            var result = await _purchaseSearch.SearchAsync(purchaseRequest.Term, purchaseRequest.Filter.GetValueOrDefault(), purchaseRequest.Location, null, token).ConfigureAwait(false);
            string nextPageLink = null;
            if (result.token != null)
            {
                nextPageLink = Url.Link("DefaultApis", new
                {
                    controller = "Places"
                }, new {nextPageToken = result.token});
            }

            return Ok(new
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
        public async Task<IHttpActionResult> Get([FromUri]string nextPageToken,
            CancellationToken token)
        {
            if (nextPageToken == null) throw new ArgumentNullException(nameof(nextPageToken));
            var result = await _purchaseSearch.SearchAsync(null,
                PlacesRequestFilter.None, null, nextPageToken, token).ConfigureAwait(false);

            string nextPageLink = null;
            if (result.token != null)
            {
                nextPageLink = Url.Link("DefaultApis", new
                {
                    controller = "Places"
                }, new { nextPageToken = result.token });
            }

            return Ok(new
            {
                nextPageLink,
                result.data
            });
        }
    }
}
