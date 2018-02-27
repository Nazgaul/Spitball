﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
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
        public async Task<IActionResult> Get([FromQuery]PurchaseRequest purchaseRequest,
            CancellationToken token)
        {
            PlacesNearbyDto retVal;
            if (!string.IsNullOrEmpty(purchaseRequest.NextPageToken))
            {
                retVal = await _purchaseSearch.SearchAsync(null,
                    PlacesRequestFilter.None, null, purchaseRequest.NextPageToken, token).ConfigureAwait(false);
            }
            else
            {
                retVal = await _purchaseSearch.SearchAsync(purchaseRequest.Term, purchaseRequest.Filter.GetValueOrDefault(), purchaseRequest.Location, null, token).ConfigureAwait(false);
            }
            string nextPageLink = null;
            if (retVal?.Token != null)
            {
                nextPageLink = Url.Link("Places", new { nextPageToken = retVal.Token });
            }

            return Ok(new
            {
                nextPageLink,
                retVal?.Data
            });
        }
    }
}
