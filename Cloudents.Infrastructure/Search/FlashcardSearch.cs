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
    public class FlashcardSearch : CseSearch, IFlashcardSearch
    {
        public async Task<(IEnumerable<SearchResult> result, string[] facet)> SearchAsync(SearchQuery model, CancellationToken token)
        {
            var term = new List<string>();
            term.AddNotNull(model.UniversitySynonym);
            term.AddNotNull(model.Course, s => '"' + s + '"');
            if (model.Query != null)
            {
                term.Add(string.Join(" ", model.Query.Select(s => '"' + s + '"')));
            }

            var result = Enumerable.Range(model.Page * 3, 3).Select(s => 
            DoSearchAsync(new Query.GoogleQuery(string.Join(" ", term), model.Source, s, model.Sort, CustomApiKey.Flashcard), token)).ToList();
            await Task.WhenAll(result).ConfigureAwait(false);
            return (

                result.Where(s => s.Result != null).SelectMany(s => s.Result),
                new[]
                {
                    "quizlet.com",
                    "cram.com",
                    "koofers.com",
                    "coursehero.com",
                    "studysoup.com",
                    "spitball.co"
                });
        }
    }
}