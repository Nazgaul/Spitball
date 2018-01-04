using System;
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
    [Route("api/Ask")]
    public class AskController : Controller
    {
        private readonly IQuestionSearch _searchProvider;
        private readonly IVideoSearch _videoSearch;

        public AskController( IQuestionSearch searchProvider, IVideoSearch videoSearch)
        {
            _searchProvider = searchProvider;
            _videoSearch = videoSearch;
        }

        public async Task<IActionResult> Get([FromQuery] AskRequest model,
            CancellationToken token)
        {
            var query = SearchQuery.Ask(model.Term,  model.Page.GetValueOrDefault());
            var tResult = _searchProvider.SearchAsync(query, token);
            var tVideo = Task.FromResult<VideoDto>(null);
            if (model.Page.GetValueOrDefault() == 0)
            {
                tVideo = _videoSearch.SearchAsync(model.Term, token);
            }
            await Task.WhenAll(tResult, tVideo).ConfigureAwait(false);
            return Json(new
            {
                result = tResult.Result,
                video = tVideo.Result
            });
        }
    }
}