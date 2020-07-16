﻿//using Cloudents.Core.DTOs.Documents;
//using Cloudents.Core.DTOs.Feed;
//using Cloudents.Core.Interfaces;
//using Cloudents.Core.Query;
//using Cloudents.Query;
//using Cloudents.Query.Documents;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Cloudents.Infrastructure.Search.Document
//{
//    public class DocumentSearch : IDocumentSearch
//    {
//        private readonly IDocumentsSearch _client;
//        private readonly IQueryBus _queryBus;
//        //private readonly IWebDocumentSearch _documentSearch;

//        public DocumentSearch(IDocumentsSearch client, IQueryBus queryBus
//            )
//        {
//            _client = client;
//            _queryBus = queryBus;
//        }

//        public async Task<IEnumerable<FeedDto>> SearchDocumentsAsync(DocumentQuery query,
//            CancellationToken token)
//        {

//            var searchResult = await _client.SearchAsync(query, query.UserProfile, token);
//            var documentResult = searchResult.result.ToList();
//            var ids = documentResult.Select(s => s.Id);
//            var queryDb = new IdsDocumentsQuery(ids);
//            var dbResult = await _queryBus.QueryAsync(queryDb, token);
//            var dic = dbResult.ToDictionary(x => x.Id);



//            var retVal = new List<DocumentFeedDto>();
//            foreach (var resultResult in documentResult)
//            {
//                if (dic.TryGetValue(resultResult.Id, out var p))
//                {
//                    //p.Source = "Cloudents";
//                    retVal.Add(p);
//                }

//            }

//            return retVal;
//        }

//    }
//}
