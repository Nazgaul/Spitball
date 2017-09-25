using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;

namespace Zbang.Cloudents.Jared.Controllers
{
    public class TutorController : ApiController
    {
        private readonly ITutorSearch m_TutorSearch;

        public TutorController(ITutorSearch tutorSearch)
        {
            m_TutorSearch = tutorSearch;
        }

        public async Task<HttpResponseMessage> Get(string term, SearchRequestFilter filter, SearchRequestSort sort, Location location, CancellationToken token)
        {
            var result = await m_TutorSearch.SearchAsync(term, filter, sort, location, token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }
    }
}
