﻿using Cloudents.Core.DTOs;
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

        public async Task<IList<DocumentFeedDto>> SearchDocumentsAsync(DocumentQuery query,
            CancellationToken token)
        {
            Task<ResultWithFacetDto<SearchResult>> taskWebResult = Task.FromResult<ResultWithFacetDto<SearchResult>>(null);
            //need to bring university Name , need to use sources
            if (!query.Filters.Any())
            {
                var webQuery = SearchQuery.Document(query.Term, query.Profile.University?.ExtraName, query.Course, query.Page);
                taskWebResult =
                    _documentSearch.SearchWithUniversityAndCoursesAsync(webQuery, HighlightTextFormat.None, token);
            }

            var searchResult = await _client.SearchAsync(query, token);
            var ids = searchResult.Results.Select(s => long.Parse(s.Document.Id));
            var queryDb = new IdsQuery<long>(ids);
            var dbResult = await _queryBus.QueryAsync<IList<DocumentFeedDto>>(queryDb, token);
            var dic = dbResult.ToDictionary(x => x.Id);



            var webResult = await taskWebResult;
            var retVal = new List<DocumentFeedDto>();
            var addedBing = false;
            foreach (var resultResult in searchResult.Results)
            {
                if (resultResult.Score - 1 < 0 && !addedBing)
                {
                    addedBing = true;
                    if (webResult?.Result != null)
                    {
                        retVal.AddRange(webResult.Result.Where(w => w != null).Select(s2 => new DocumentFeedDto()
                        {
                            Snippet = s2.Snippet,
                            Title = s2.Title,
                            Url = s2.Url,
                            Source = s2.Source
                        }));
                    }
                }
                if (dic.TryGetValue(long.Parse(resultResult.Document.Id), out var p))
                {
                    p.Snippet = resultResult.Document.MetaContent;
                    p.Source = "Cloudents";
                    //p.Url = _urlBuilder.BuildDocumentEndPoint(p.Id);
                    retVal.Add(p);
                }

            }

            if (!addedBing)
            {
                if (webResult?.Result != null)
                {
                    retVal.AddRange(webResult.Result.Where(w => w != null).Select(s2 => new DocumentFeedDto()
                    {
                        Snippet = s2.Snippet,
                        Title = s2.Title,
                        Url = s2.Url,
                        Source = s2.Source
                    }));
                }
            }
            return retVal;
            //var retVal = new QuestionWithFacetDto { Result = dbResult };
        }

    }
}
