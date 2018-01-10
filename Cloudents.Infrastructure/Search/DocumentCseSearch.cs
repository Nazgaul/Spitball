using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;

namespace Cloudents.Infrastructure.Search
{
    public class DocumentCseSearch : IDocumentCseSearch
    {
        private readonly ISearch _search;
        private readonly ISearchConvertRepository _searchConvertRepository;

        public DocumentCseSearch(ISearch search, ISearchConvertRepository searchConvertRepository)
        {
            _search = search;
            _searchConvertRepository = searchConvertRepository;
        }

        public async Task<ResultWithFacetDto<SearchResult>> SearchAsync(SearchQuery model, CancellationToken token)
        {
            var (universitySynonym, courses) = await _searchConvertRepository.ParseUniversityAndCoursesAsync(model.University, model.Courses, token).ConfigureAwait(false);
            var cseModel = new SearchModel(model.Query, model.Source, model.Page, model.Sort, CustomApiKey.Documents,courses,universitySynonym,"biology",model.DocType);
            var result = await _search.DoSearchAsync(cseModel, token);

            return new ResultWithFacetDto<SearchResult>
            {
                Result = result,
                Facet = new[]
                {
                    "Spitball.co",
                    "Studysoup.com",
                    "Coursehero.com",
                    "Cliffsnotes.com",
                    "Oneclass.com",
                    "Koofers.com",
                }
            };
        }
    }
}
