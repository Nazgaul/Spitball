using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;

namespace Cloudents.Infrastructure.Search
{
    public class DocumentSearch : IDocumentSearch
    {
        private readonly ICseSearch m_Search;

        public DocumentSearch(ICseSearch search)
        {
            m_Search = search;
        }

        public async Task<ResultWithFacetDto<SearchResult>> SearchAsync(SearchQuery model, CancellationToken token)
        {
            var term = new List<string>();

            term.AddNotNull(model.UniversitySynonym);
            term.AddNotNull(model.Course, s => '"' + s + '"');
            if (model.Query != null)
            {
                term.Add(string.Join(" ", model.Query.Select(s => '"' + s + '"')));
            }
            
            var result = Enumerable.Range(model.Page * 3, 3).Select(s => m_Search.DoSearchAsync(string.Join(" ", term), model.Source, s, model.Sort,
                CustomApiKey.Documents,
                token)).ToList();
            await Task.WhenAll(result).ConfigureAwait(false);
            return new ResultWithFacetDto<SearchResult>
            {
                Result = result.Where(s => s.Result != null).SelectMany(s => s.Result),
                Facet = new[]
                {
                    "uloop.com",
                    "spitball.co",
                    "studysoup.com",
                    "coursehero.com",
                    "cliffsnotes.com",
                    "oneclass.com",
                    "koofers.com",
                    "studylib.net"
                }
            };
        }
    }
}
