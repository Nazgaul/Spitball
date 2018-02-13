using System;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.MobileApi.Extensions;
using Cloudents.MobileApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.MobileApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// The food and places api controller
    /// </summary>
    [Route("api/[controller]",Name ="Places")]
    public class PlacesController : Controller
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
        [HttpGet]
        public  Task<IActionResult> Get([FromQuery]PurchaseRequest purchaseRequest,
            CancellationToken token)
        {
            if (!string.IsNullOrEmpty(purchaseRequest.NextPageToken))
            {
                return PagingAsync(purchaseRequest.NextPageToken, token);
            }
            return InitCallAsync(purchaseRequest, token);
        }


        private async Task<IActionResult> InitCallAsync(PurchaseRequest purchaseRequest,
            CancellationToken token)
        {
            var result = await _purchaseSearch.SearchAsync(purchaseRequest.Term, purchaseRequest.Filter.GetValueOrDefault(), purchaseRequest.Location, null, token).ConfigureAwait(false);
            string nextPageLink = null;
            if (result.token != null)
            {
                nextPageLink = Url.Link("Places", new { nextPageToken = result.token });
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
        //[HttpGet]
        private async Task<IActionResult> PagingAsync(string nextPageToken,
            CancellationToken token)
        {
            if (nextPageToken == null) throw new ArgumentNullException(nameof(nextPageToken));
            var result = await _purchaseSearch.SearchAsync(null,
                PlacesRequestFilter.None, null, nextPageToken, token).ConfigureAwait(false);

            var nextPageLink = Url.Link("Places", new { nextPageToken = result.token });

            return Ok(new
            {
                nextPageLink,
                result.data
            });
        }
    }
}
