using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;

namespace Cloudents.Infrastructure.Search
{
    public class FlashcardSearch : IFlashcardSearch
    {
        private readonly ICseSearch _search;
        private readonly ISearchConvertRepository _searchConvertRepository;

        public FlashcardSearch(ICseSearch search, ISearchConvertRepository searchConvertRepository)
        {
            _search = search;
            _searchConvertRepository = searchConvertRepository;
        }

        public async Task<ResultWithFacetDto<SearchResult>> SearchAsync(SearchQuery model, CancellationToken token)
        {
            var (universitySynonym, courses) = await _searchConvertRepository.ParseUniversityAndCoursesAsync(model.University, model.Courses, token).ConfigureAwait(false);

            var term = new List<string>();

            if (universitySynonym != null)
            {
                term.Add(string.Join(" OR ", universitySynonym.Select(s => '"' + s + '"')));
            }
            if (courses != null)
            {
                term.Add(string.Join(" OR ", courses.Select(s => '"' + s.UppercaseFirst() + '"')));
            }
            if (model.Query != null)
            {
                term.AddNotNull(string.Join(" ", model.Query));
            }
            if (term.Count == 0)
            {
                term.Add("accounting");
            }

            var result = Enumerable.Range(model.Page * CseSearch.NumberOfPagesPerRequest, CseSearch.NumberOfPagesPerRequest).Select(s =>
            {
                var cseModel = new CseModel(term, model.Source, s, model.Sort, CustomApiKey.Flashcard);
                return _search.DoSearchAsync(cseModel,
                    token);
            }).ToList();

            //var result = Enumerable.Range(model.Page * 3, 3).Select(s => { }
            //    m_Search.DoSearchAsync(string.Join(" ", term), model.Source, s, model.Sort, CustomApiKey.Flashcard, token)).ToList();
            await Task.WhenAll(result).ConfigureAwait(false);
            return new ResultWithFacetDto<SearchResult>
            {

                Result = result.Where(s => s.Result != null).SelectMany(s => s.Result),
                Facet = new[]
                {
                    "Quizlet.com",
                    "Cram.com",
                    "Koofers.com",
                    "Coursehero.com",
                    "Studysoup.com",
                    "Spitball.co"
                }

            };
        }
    }
}