using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Api.Extensions;
using Cloudents.Api.Models;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Read;
using Cloudents.Core.Request;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Api.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Perform Ask question search
    /// </summary>
    [Route("api/[controller]", Name = "Ask")]
    public class AskController : Controller
    {
        private readonly WebSearch _searchProvider;
        private readonly IVideoSearch _videoSearch;

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="searchFactory"></param>
        /// <param name="videoSearch"></param>
        public AskController(WebSearch.Factory searchFactory, IVideoSearch videoSearch)
        {
            _searchProvider = searchFactory.Invoke(CustomApiKey.AskQuestion);
            _videoSearch = videoSearch;
        }

        /// <summary>
        /// Query to get ask vertical
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] AskRequest model,
            CancellationToken token)
        {
            var query = SearchQuery.Ask(model.Term, model.Page.GetValueOrDefault(), model.Source);
            var tResult = _searchProvider.SearchAsync(query, model.Format, token);
            var tVideo = Task.FromResult<VideoDto>(null);
            if (model.Page.GetValueOrDefault() == 0)
            {
                tVideo = _videoSearch.SearchAsync(model.Term, token);
            }
            await Task.WhenAll(tResult, tVideo).ConfigureAwait(false);

            string nextPageLink = null;
            if (tResult.Result.Result?.Any() == true)
            {
                nextPageLink = Url.NextPageLink("Ask", model);
            }

            return Ok(new
            {
                result = tResult.Result,
                video = tVideo.Result,
                nextPageLink
            });
        }
    }
}