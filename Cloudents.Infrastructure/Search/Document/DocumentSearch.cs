using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Core.Request;
using Cloudents.Query;
using Cloudents.Query.Query;

namespace Cloudents.Infrastructure.Search.Document
{
    public class DocumentSearch : IDocumentSearch
    {
        private readonly IDocumentsSearch _client;
        private readonly IQueryBus _queryBus;
        private readonly IWebDocumentSearch _documentSearch;

        public DocumentSearch(IDocumentsSearch client, IQueryBus queryBus, IWebDocumentSearch documentSearch
            )
        {
            _client = client;
            _queryBus = queryBus;
            _documentSearch = documentSearch;
        }

        //public Task<string> ItemContentAsync(long itemId, CancellationToken cancelToken)
        //{
        //    return _client.ItemContentAsync(itemId, cancelToken);
        //}

        //public Task<string> ItemMetaContentAsync(long itemId, CancellationToken cancelToken)
        //{
        //    return _client.ItemMetaContentAsync(itemId, cancelToken);
        //}

        public async Task<IList<DocumentFeedDto>> SearchDocumentsAsync(DocumentQuery query,
            CancellationToken token)
        {
            

            var searchResult = (await _client.SearchAsync(query, token)).ToList();
            var ids = searchResult.Select(s => s.Id);
            var queryDb = new IdsQuery<long>(ids);
            var dbResult = await _queryBus.QueryAsync<IList<DocumentFeedDto>>(queryDb, token);
            var dic = dbResult.ToDictionary(x => x.Id);



            var retVal = new List<DocumentFeedDto>();
            foreach (var resultResult in searchResult)
            {
                if (dic.TryGetValue(resultResult.Id, out var p))
                {
                    p.Source = "Cloudents";
                    retVal.Add(p);
                }

            }

            if (retVal.Count == 0)
            {
                //need to bring university Name , need to use sources
                if (!query.Filters.Any())
                {
                    //case 12148
                    var webQuery = SearchQuery.Document(query.Term, query.Profile.University?.ExtraName, query.Course, query.Page);
                    var webResult = await 
                        _documentSearch.SearchWithUniversityAndCoursesAsync(webQuery, HighlightTextFormat.None, token);
                    if (webResult.Result != null)
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
            }
            return retVal;
        }

    }
}
