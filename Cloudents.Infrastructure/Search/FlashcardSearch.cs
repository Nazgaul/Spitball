using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;

namespace Cloudents.Infrastructure.Search
{
    public class FlashcardSearch : IFlashcardSearch
    {
        private readonly ISearch _search;
        private readonly ISearchConvertRepository _searchConvertRepository;

        public FlashcardSearch(ISearch search, ISearchConvertRepository searchConvertRepository)
        {
            _search = search;
            _searchConvertRepository = searchConvertRepository;
        }

        public async Task<ResultWithFacetDto<SearchResult>> SearchAsync(SearchQuery model, BingTextFormat format, CancellationToken token)
        {
            var (universitySynonym, courses) = await _searchConvertRepository.ParseUniversityAndCoursesAsync(model.University, model.Courses, token).ConfigureAwait(false);

            var cseModel = new SearchModel(model.Query, model.Source, model.Page, model.Sort, CustomApiKey.Flashcard, courses, universitySynonym, "accounting", null);
            var result = await _search.DoSearchAsync(cseModel, format, token).ConfigureAwait(false);
            return new ResultWithFacetDto<SearchResult>
            {
                Result = result.ToList(),
                Facet = new[]
                {
                    "Quizlet.com",
                    "Cram.com",
                    "Koofers.com",
                    "Coursehero.com",
                    "Studysoup.com",
                    "Spitball.co",
                    "Studyblue.com"
                }

            };
        }
    }
}