using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;

namespace Cloudents.Core.Read
{
    public class WebSearch
    {
        public delegate WebSearch Factory(CustomApiKey api);
        private readonly ISearch _search;
        private readonly ISearchConvertRepository _searchConvertRepository;
        private readonly CustomApiKey _api;
        

        public WebSearch(ISearch search, ISearchConvertRepository searchConvertRepository, CustomApiKey api)
        {
            _search = search;
            _searchConvertRepository = searchConvertRepository;
            _api = api;
        }

        

        public async Task<ResultWithFacetDto<SearchResult>> SearchWithUniversityAndCoursesAsync(SearchQuery model, HighlightTextFormat format, CancellationToken token)
        {
            var (universitySynonym, courses) = await _searchConvertRepository.ParseUniversityAndCoursesAsync(model.University, model.Courses, token).ConfigureAwait(false);

            var cseModel = new SearchModel(model.Query, model.Source, _api, courses, universitySynonym, model.DocType);
            var result = await _search.SearchAsync(cseModel, model.Page, format, token).ConfigureAwait(false);
            var facets = _api.Priority.Select(s => s.Key);
            return new ResultWithFacetDto<SearchResult>
            {
                Result = result,
                Facet = facets

            };
        }

        public async Task<ResultWithFacetDto<SearchResult>> SearchAsync(SearchQuery model, HighlightTextFormat format, CancellationToken token)
        {

            var cseModel = new SearchModel(model.Query, model.Source, _api, null, null, model.DocType);
            var result = await _search.SearchAsync(cseModel, model.Page, format, token).ConfigureAwait(false);
            var facets = _api.Priority.Select(s => s.Key);
            return new ResultWithFacetDto<SearchResult>
            {
                Result = result,
                Facet = facets

            };
        }
    }
}