using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Request;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Tutor")]
    public class TutorController : Controller
    {
        private readonly ITutorSearch m_TutorSearch;

        public TutorController(ITutorSearch tutorSearch)
        {
            m_TutorSearch = tutorSearch;
        }
        public async Task<IActionResult> Get(string[] term, SearchRequestFilter filter, SearchRequestSort sort, GeoPoint location, CancellationToken token)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));
            var result = await m_TutorSearch.SearchAsync(string.Join(" ", term), filter, sort, location, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}