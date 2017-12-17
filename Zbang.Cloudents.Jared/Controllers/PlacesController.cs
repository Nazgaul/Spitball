﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Mobile.Server.Config;
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
        public async Task<HttpResponseMessage> Get(PurchaseRequest purchaseRequest,
            CancellationToken token)
        {
            if (purchaseRequest.Term == null) throw new ArgumentNullException(nameof(purchaseRequest.Term));
            if (purchaseRequest.Location == null) throw new ArgumentNullException(nameof(purchaseRequest.Location));
            var result = await _purchaseSearch.SearchNearbyAsync(string.Join(" ", purchaseRequest.Term), purchaseRequest.Filter.GetValueOrDefault(), purchaseRequest.Location, null, token).ConfigureAwait(false);
            return Request.CreateResponse(new
            {
                result.token,result.data
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
            return Request.CreateResponse(new
            {
                result.token,
                result.data
            });
        }
    }
}
