using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Places")]
    public class PlacesController : Controller
    {
        private readonly IPlacesSearch _placesSearch;

        public PlacesController(IPlacesSearch placesSearch)
        {
            _placesSearch = placesSearch;
        }

        [TypeFilter(typeof(IpToLocationActionFilter),Arguments = new object[] {"location"})]
        [HttpGet]
        public async Task<IActionResult> GetAsync(string[] term, PlacesRequestFilter filter, GeoPoint location, CancellationToken token)
        {
            if (location == null) throw new ArgumentNullException(nameof(location));
            var queryTerm = string.Join(" ", term ?? Enumerable.Empty<string>());
            var result = await _placesSearch.SearchNearbyAsync(queryTerm, filter, location, default, token).ConfigureAwait(false);
            return Json(new
            {
                result.token,
                result.data
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([RequiredFromQuery]string nextPageToken,
            CancellationToken token)
        {
            if (nextPageToken == null) throw new ArgumentNullException(nameof(nextPageToken));
            var result = await _placesSearch.SearchNearbyAsync(string.Empty,
                default, default, nextPageToken, token).ConfigureAwait(false);

            return Json(new
            {
                result.token,
                result.data
            });
        }
    }
}