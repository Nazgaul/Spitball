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
    [Route("api/Title")]
    public class TitleController : Controller
    {
        private readonly ITitleSearch _titleSearch;

        public TitleController(ITitleSearch titleSearch)
        {
            _titleSearch = titleSearch;
        }

        public async Task<IActionResult> Get(string term, CancellationToken token)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));
            var result = await _titleSearch.SearchAsync(term, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}