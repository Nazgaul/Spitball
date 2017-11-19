using System;
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
    [Route("api/Ask")]
    public class AskController : Controller
    {
        private readonly Lazy<IReadRepositoryAsync<UniversitySynonymDto, long>> _universitySynonymRepository;
        private readonly IQuestionSearch _searchProvider;
        private readonly IVideoSearch _videoSearch;

        public AskController(Lazy<IReadRepositoryAsync<UniversitySynonymDto, long>> universitySynonymRepository, IQuestionSearch searchProvider, IVideoSearch videoSearch)
        {
            _universitySynonymRepository = universitySynonymRepository;
            _searchProvider = searchProvider;
            _videoSearch = videoSearch;
        }

        public async Task<IActionResult> Get([FromQuery] AskRequest model,
            CancellationToken token)
        {
            string universitySynonym = null;
            if (model.University.HasValue && !string.IsNullOrEmpty(model.Course))
            {
                var repositoryResult = await _universitySynonymRepository.Value.GetAsync(model.University.Value, token).ConfigureAwait(false);
                universitySynonym = repositoryResult.Name;
            }
            var query = new SearchQuery(model.Term, universitySynonym, model.Course, model.Source, model.Page.GetValueOrDefault(),
                model.Sort.GetValueOrDefault());
            var tResult = _searchProvider.SearchAsync(query, token);
            var tVideo = Task.FromResult<VideoDto>(null);
            if (model.Page.GetValueOrDefault() == 0)
            {
                tVideo = _videoSearch.SearchAsync(model.UserText, token);
            }
            await Task.WhenAll(tResult, tVideo).ConfigureAwait(false);
            return Json(new
            {
                result = tResult.Result.Take(1),
                video = tVideo.Result
            });
        }
    }
}