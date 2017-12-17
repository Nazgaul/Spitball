using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.Jared.Models;

namespace Zbang.Cloudents.Jared.Controllers
{
    /// <summary>
    /// Perform Ask question search
    /// </summary>
    [MobileAppController]
    public class AskController : ApiController
    {
        private readonly IQuestionSearch _searchProvider;
        private readonly IVideoSearch _videoSearch;

        public AskController( IQuestionSearch searchProvider, IVideoSearch videoSearch)
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
        public async Task<HttpResponseMessage> Get([FromUri] AskRequest model,
            CancellationToken token)
        {
            var query = new SearchQuery(model.Term,  model.Page.GetValueOrDefault());
            var tResult = _searchProvider.SearchAsync(query, token);
            var tVideo = Task.FromResult<VideoDto>(null);
            if (model.Page.GetValueOrDefault() == 0)
            {
                tVideo = _videoSearch.SearchAsync(model.UserText, token);
            }
            await Task.WhenAll(tResult, tVideo).ConfigureAwait(false);
            return Request.CreateResponse(new
            {
                result = tResult.Result,
                video = tVideo.Result
            });
           
        }
    }
}