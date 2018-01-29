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
    [Route("api/Tutor")]
    public class TutorController : Controller
    {
        private readonly ITutorSearch _tutorSearch;

        public TutorController(ITutorSearch tutorSearch)
        {
            _tutorSearch = tutorSearch;
        }

        [TypeFilter(typeof(IpToLocationActionFilter), Arguments = new object[] { "location" })]
        public async Task<IActionResult> Get(string[] term,
            TutorRequestFilter[] filter,
            TutorRequestSort? sort,
            Location location, int page, CancellationToken token)
        {
            var result = await _tutorSearch.SearchAsync(term, filter, sort.GetValueOrDefault(TutorRequestSort.Price), location.Point, page, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}