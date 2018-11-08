using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Core.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Search.Document
{
    public class DocumentSearch : IDocumentSearch
    {
        private readonly AzureDocumentSearch _client;
        private readonly IQueryBus _queryBus;
        private readonly IWebDocumentSearch _documentSearch;

        public DocumentSearch(AzureDocumentSearch client, IQueryBus queryBus, IWebDocumentSearch documentSearch
            )
        {
            _client = client;
            _queryBus = queryBus;
            _documentSearch = documentSearch;
        }

        public Task<string> ItemContentAsync(long itemId, CancellationToken cancelToken)
        {
            return _client.ItemContentAsync(itemId, cancelToken);
        }

        public Task<string> ItemMetaContentAsync(long itemId, CancellationToken cancelToken)
        {
            return _client.ItemMetaContentAsync(itemId, cancelToken);
        }

        public async Task<ResultWithFacetDto2<DocumentFeedDto>> SearchDocumentsAsync(DocumentQuery query,
            CancellationToken token)
        {
            //need to bring university Name , need to use sources
            var webQuery = SearchQuery.Document(query.Term, query.University, query.Course, query.Sources, query.Page);
            var taskWebResult = _documentSearch.SearchWithUniversityAndCoursesAsync(webQuery, HighlightTextFormat.None, token);

            var searchResult = await _client.SearchAsync(query, token);
            var ids = searchResult.Results.Select(s => long.Parse(s.Document.Id));
            var queryDb = new IdsQuery<long>(ids);
            var dbResult = await _queryBus.QueryAsync<IList<DocumentFeedDto>>(queryDb, token);
            var dic = dbResult.ToDictionary(x => x.Id);



            var webResult = await taskWebResult;
            var retVal = new ResultWithFacetDto2<DocumentFeedDto>();

            foreach (var resultResult in searchResult.Results)
            {
                if (Math.Abs(resultResult.Score - 1) < 0.01)
                {
                    //retVal.Result.AddRange(webResult.Result.Where(w => w != null).Select(s2 => new DocumentFeedDto()
                    //{
                    //    Snippet = s2.Snippet,
                    //    Title = s2.Title,
                    //    Url = s2.Url,
                    //    Source = s2.Source
                    //}));
                }
                if (dic.TryGetValue(long.Parse(resultResult.Document.Id), out var p))
                {
                    p.Snippet = resultResult.Document.MetaContent;
                    p.Source = "Cloudents";
                    //p.Url = _urlBuilder.BuildDocumentEndPoint(p.Id);
                    retVal.Result.Add(p);
                }

            }

            retVal.Facet = webResult.Facet;
            return retVal;
            //var retVal = new QuestionWithFacetDto { Result = dbResult };
        }

    }
}
