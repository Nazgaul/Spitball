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

        public FlashcardSearch(ICseSearch search)
        {
            _search = search;
        }

        public async Task<ResultWithFacetDto<SearchResult>> SearchAsync(SearchQuery model, CancellationToken token)
        {
            var term = new List<string>();

            if (model.UniversitySynonym != null)
            {
                term.Add(string.Join(" OR ", model.UniversitySynonym.Select(s => '"' + s + '"')));
            }
            if (model.Course != null)
            {
                term.Add(string.Join(" OR ", model.Course.Select(s => '"' + s + '"')));
            }
            if (model.Query != null)
            {
                term.Add(string.Join(" ", model.Query));
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