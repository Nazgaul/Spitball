using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Web.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Places")]
    public class PlacesController : Controller
    {
        private readonly IPlacesSearch m_PlacesSearch;

        public PlacesController(IPlacesSearch placesSearch)
        {
            m_PlacesSearch = placesSearch;
        }

        [TypeFilter(typeof(IpToLocationActionFilter),Arguments = new object[] {"location"})]
        [HttpGet(/*"{term:requiredArray}"*/)]
        public async Task<IActionResult> Get(string[] term, SearchRequestFilter filter, GeoPoint location, CancellationToken token)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));
            if (location == null) throw new ArgumentNullException(nameof(location));
            var result = await m_PlacesSearch.SearchNearbyAsync(string.Join(" ", term), filter, location, default, token).ConfigureAwait(false);
            return Json(new
            {
                result.token,
                result.data
            });
        }

        [HttpGet("{nextPageToken:required}")]
        public async Task<IActionResult> Get(string nextPageToken,
            CancellationToken token)
        {
            if (nextPageToken == null) throw new ArgumentNullException(nameof(nextPageToken));
            var result = await m_PlacesSearch.SearchNearbyAsync(string.Empty,
                default, default, nextPageToken, token).ConfigureAwait(false);

            return Json(new
            {
                result.token,
                result.data
            });
        }
    }
}