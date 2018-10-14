using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.Azure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Document = Cloudents.Core.Entities.Search.Document;
using SearchResult = Cloudents.Core.DTOs.SearchResult;

namespace Cloudents.Infrastructure.Search
{
    public class DocumentIndexSearch : IDocumentSearch
    {
        private readonly ISearchIndexClient _client;

        public DocumentIndexSearch(ISearchService client)
        {
            _client = client.GetOldClient("item3");

        }

        public async Task<string> ItemContentAsync(long itemId, CancellationToken cancelToken)
        {
            try
            {
                var item =
                    await
                        _client.Documents.GetAsync<Document>
                        (itemId.ToString(CultureInfo.InvariantCulture),
                            new[] { "content" }, cancellationToken: cancelToken).ConfigureAwait(false);
                return item.Content;
            }
            //item may not exists in the search....
            catch (CloudException)
            {
                return null;
            }
        }

        private static string GetFieldName(Expression<Func<Document, object>> memberExpression)
        {
            return memberExpression.GetName().ToCamelCase();
        }

        public async Task<ResultWithFacetDto<SearchResult>> SearchDocumentsAsync(SearchQuery query,
            CancellationToken cancelToken)
        {

            var listFilterExpression = new List<string>();
            if (query.University.HasValue)
            {
                listFilterExpression.Add($"({GetFieldName(x => x.UniversityId)} eq '{query.University.Value}')");
            }

            if (query.Courses != null)
            {
                listFilterExpression.AddRange(query.Courses.Select(course =>
                    $"({GetFieldName(x => x.BoxId2)} eq {course})"));
            }


            var searchParameters = new SearchParameters()
            {
                Select = new[]
                    {
                    GetFieldName(x=>x.Id),
                    GetFieldName(x=>x.Image),
                    GetFieldName(x=>x.Name),
                    GetFieldName(x=>x.MetaContent),
                    GetFieldName(x=>x.Url)
                },
                Top = 50,
                Skip = 50 * query.Page,
                Filter = string.Join("and", listFilterExpression)

            };

            var retVal = new ResultWithFacetDto<SearchResult>();
            var result = await _client.Documents.SearchAsync<Document>(query.Query, searchParameters, cancellationToken: cancelToken);
            retVal.Result = result.Results.Select(s => new SearchResult
            {
                Id = s.Document.Id,
                Image = s.Document.Image,
                Title = s.Document.Name,
                Snippet = s.Document.MetaContent,
                Url = BuildUrlWithIsNew(s.Document.Url),
                Source = "Spitball"
            });
            return retVal;
        }

        private static string BuildUrlWithIsNew(string x)
        {
            return x + "?isNew=true";
        }
    }
}
