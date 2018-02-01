using System;
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

       // [TypeFilter(typeof(IpToLocationActionFilter),Arguments = new object[] {"location"})]
        [HttpGet]
        public async Task<IActionResult> GetAsync(string[] term, PlacesRequestFilter filter, Location location, CancellationToken token)
        {
            if (location == null) throw new ArgumentNullException(nameof(location));
            var result = await _placesSearch.SearchAsync(term, filter, location.Point, null, token).ConfigureAwait(false);
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
            var result = await _placesSearch.SearchAsync(null,
                PlacesRequestFilter.None, null, nextPageToken, token).ConfigureAwait(false);

            return Json(new
            {
                result.token,
                result.data
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetByIdAsync([RequiredFromQuery]string id,
            CancellationToken token, [FromServices] IGooglePlacesSearch places)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            var result = await places.ByIdAsync(id, token).ConfigureAwait(false);

            return Json(result);
        }
    }
}