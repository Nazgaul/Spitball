using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Query;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Query.Documents;

namespace Cloudents.Infrastructure.Search.Document
{
    public class DocumentSearch : IDocumentSearch
    {
        private readonly IDocumentsSearch _client;
        private readonly IQueryBus _queryBus;
        //private readonly IWebDocumentSearch _documentSearch;

        public DocumentSearch(IDocumentsSearch client, IQueryBus queryBus
            )
        {
            _client = client;
            _queryBus = queryBus;
        }

        public async Task<DocumentFeedWithFacetDto> SearchDocumentsAsync(DocumentQuery query,
            CancellationToken token)
        {

            var searchResult = await _client.SearchAsync(query, query.UserProfile, token);
            var documentResult = searchResult.result.ToList();
            var ids = documentResult.Select(s => s.Id);
            var queryDb = new IdsDocumentsQuery(ids);
            var dbResult = await _queryBus.QueryAsync(queryDb, token);
            var dic = dbResult.ToDictionary(x => x.Id);



            var retVal = new List<DocumentFeedDto>();
            foreach (var resultResult in documentResult)
            {
                if (dic.TryGetValue(resultResult.Id, out var p))
                {
                    //p.Source = "Cloudents";
                    retVal.Add(p);
                }

            }

            if (retVal.Count == 0)
            {
                //need to bring university Name , need to use sources
                //if (!query.Filters?.Any() == true)
                //{
                //    //case 12148
                //    var webQuery = BingSearchQuery.Document(query.Term, query.UserProfile.University?.ExtraName,
                //        query.Course, query.Page);
                //    var webResult = await
                //        _documentSearch.SearchWithUniversityAndCoursesAsync(webQuery, token);
                //    if (webResult.Result != null)
                //    {
                //        retVal.AddRange(webResult.Result.Where(w => w != null).Select(s2 => new DocumentFeedDto()
                //        {
                //            Snippet = s2.Snippet,
                //            Title = s2.Title,
                //            Url = s2.Url,
                //            Source = s2.Source
                //        }));
                //    }
                //}
            }

            return new DocumentFeedWithFacetDto
            {
                Result = retVal,
                Facet = searchResult.facetSubject
            };
        }

    }
}
