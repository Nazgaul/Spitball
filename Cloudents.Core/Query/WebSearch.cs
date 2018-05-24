using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using JetBrains.Annotations;

namespace Cloudents.Core.Query
{
    [UsedImplicitly]
    public class WebSearch : IWebDocumentSearch, IWebFlashcardSearch
    {
        private readonly ISearch _search;
        private readonly ISearchConvertRepository _searchConvertRepository;
        private readonly IUniversitySearch _universitySearch;
        private readonly CustomApiKey _api;



        public WebSearch(ISearch search, ISearchConvertRepository searchConvertRepository, CustomApiKey api, IUniversitySearch universitySearch)
        {
            _search = search;
            _searchConvertRepository = searchConvertRepository;
            _api = api;
            _universitySearch = universitySearch;
        }

        public async Task<ResultWithFacetDto<SearchResult>> SearchWithUniversityAndCoursesAsync(SearchQuery model, HighlightTextFormat format, CancellationToken token)
        {
            var university = model.University;
            if (!model.University.HasValue && model.Point != null)
            {
                var p = await _universitySearch.GetApproximateUniversitiesAsync(model.Point, token).ConfigureAwait(false);
                university = p?.Id;
            }
            var (universitySynonym, courses) = await _searchConvertRepository.ParseUniversityAndCoursesAsync(university, model.Courses, token).ConfigureAwait(false);

            var cseModel = new SearchModel(model.Query, BuildSources(model.Source), _api, courses, universitySynonym, model.DocType);
            var result = await _search.SearchAsync(cseModel, model.Page, format, token).ConfigureAwait(false);
            var facets = _api.Priority.Select(s => s.Key).OrderBy(s => s);
            return new ResultWithFacetDto<SearchResult>
            {
                Result = result,
                Facet = facets

            };
        }

        private IEnumerable<string> BuildSources(IEnumerable<string> sources)
        {
            return sources?.Select(s =>
            {
                if (_api.Priority.TryGetValue(s, out var notes))
                {
                    return notes?.Domains;
                }

                return null;
            }).Where(w => w != null).SelectMany(s => s);
        }
        
    }
}