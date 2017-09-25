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
    [Route("api/Video")]
    public class VideoController : Controller
    {
        private readonly IVideoSearch m_VideoSearch;

        public VideoController(IVideoSearch videoSearch)
        {
            m_VideoSearch = videoSearch;
        }

        public async Task<IActionResult> Get(string term, CancellationToken token)
        {
            var result = await m_VideoSearch.SearchAsync(term, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}