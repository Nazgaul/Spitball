using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Places")]
    public class PlacesController : Controller
    {
        private readonly IPurchaseSearch m_PurchaseSearch;

        public PlacesController(IPurchaseSearch purchaseSearch)
        {
            m_PurchaseSearch = purchaseSearch;
        }


        public async Task<IActionResult> Get(string term, SearchRequestFilter filter, GeoPoint location, CancellationToken token)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));
            if (location == null) throw new ArgumentNullException(nameof(location));
            var result = await m_PurchaseSearch.SearchAsync(term, filter, location, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}