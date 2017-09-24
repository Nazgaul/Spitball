using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
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

        public async Task<IActionResult> Get(string term, bool isOnline, bool inPerson, CancellationToken token)
        {
            var result = await m_TutorSearch.SearchAsync(term, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}