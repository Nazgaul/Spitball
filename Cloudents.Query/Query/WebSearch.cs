//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Application.DTOs;
//using Cloudents.Application.Enum;
//using Cloudents.Application.Interfaces;
//using Cloudents.Application.Request;
//using JetBrains.Annotations;

//namespace Cloudents.Application.Query
//{
//    [UsedImplicitly]
//    public class WebSearch : IWebDocumentSearch, IWebFlashcardSearch
//    {
//        private readonly ISearch _search;
//        private readonly CustomApiKey _api;

//        public WebSearch(ISearch search,  CustomApiKey api)
//        {
//            _search = search;
//            _api = api;
//        }

//        public async Task<ResultWithFacetDto<SearchResult>> SearchWithUniversityAndCoursesAsync(SearchQuery model,
//            HighlightTextFormat format, CancellationToken token)
//        {
//            //var queryDb = new UniversityCoursesSynonymQuery(model.University, model.Courses);
//            //var resultDb = await _queryBus.QueryAsync(queryDb, token);

//            var cseModel = new SearchModel(model.Query, BuildSources(model.Source),
//                _api, model.Courses, model.University);
//            var result = await _search.SearchAsync(cseModel, model.Page, format, token).ConfigureAwait(false);
//            var facets = _api.Priority.Select(s => s.Key).OrderBy(s => s);
//            return new ResultWithFacetDto<SearchResult>
//            {
//                Result = result,
//                Facet = facets

//            };
//        }

//        private IEnumerable<string> BuildSources(IEnumerable<string> sources)
//        {
//            return sources?.Select(s =>
//            {
//                if (_api.Priority.TryGetValue(s, out var notes))
//                {
//                    return notes?.Domains;
//                }

//                return null;
//            }).Where(w => w != null).SelectMany(s => s);
//        }
//    }
//}