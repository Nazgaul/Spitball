﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Search")]
    public class SearchController : Controller
    {

        [Route("documents")]
        public async Task<IActionResult> SearchDocumentAsync([FromQuery] DocumentSearchRequest model,
            CancellationToken token, [FromServices] IDocumentCseSearch searchProvider)
        {
            var term = model.Term.Union(new[] {model.DocType});
            var query = new SearchQuery(term, model.University, model.Course, model.Source, model.Page.GetValueOrDefault(),
                model.Sort.GetValueOrDefault());

            var result = await searchProvider.SearchAsync(query, token).ConfigureAwait(false);
            return Json(result);
        }


        [Route("flashcards")]
        public async Task<IActionResult> SearchFlashcardsAsync([FromQuery] SearchRequest model,
            CancellationToken token, [FromServices] IFlashcardSearch searchProvider)
        {
            var query = new SearchQuery(model.Term, model.University, model.Course, model.Source, model.Page.GetValueOrDefault(),
                model.Sort.GetValueOrDefault());

            var result = await searchProvider.SearchAsync(query, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}