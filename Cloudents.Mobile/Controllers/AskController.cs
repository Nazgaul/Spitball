using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Mobile.Extensions;
using Cloudents.Mobile.Models;
using Microsoft.Azure.Mobile.Server.Config;

namespace Cloudents.Mobile.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Perform Ask question search
    /// </summary>
    [MobileAppController]
    public class AskController : ApiController
    {
        private readonly IQuestionSearch _searchProvider;
        private readonly IVideoSearch _videoSearch;

        /// <inheritdoc />
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="searchProvider"></param>
        /// <param name="videoSearch"></param>
        public AskController(IQuestionSearch searchProvider, IVideoSearch videoSearch)
        {
            _searchProvider = searchProvider;
            _videoSearch = videoSearch;
        }

        /// <summary>
        /// Query to get ask vertical
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> Get([FromUri] AskRequest model,
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
            var nextPageLink = Url.NextPageLink("DefaultApis", new
            {
                controller = "Ask"
            }, model);

            return Ok(new
            {
                result = tResult.Result,
                video = tVideo.Result,
                nextPageLink
            });
        }
    }
}