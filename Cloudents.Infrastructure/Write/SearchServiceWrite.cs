using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Search;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Write
{
    public abstract class SearchServiceWrite<T> : IDisposable, ISearchServiceWrite<T> where T : class, ISearchObject, new()
    {
        private readonly SearchServiceClient _client;
        protected readonly ISearchIndexClient IndexClient;
       // private readonly string _indexName;

        protected SearchServiceWrite(SearchService client, string indexName) 
            :this(client, client.GetOldClient(indexName))

        {
           
        }

        protected SearchServiceWrite(SearchService client, ISearchIndexClient indexClient)
        {
            _client = client.Client;
            IndexClient = indexClient;
        }


        public async Task<bool> UpdateDataAsync(IEnumerable<T> items, CancellationToken token)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            var batch = IndexBatch.MergeOrUpload(items);
            var result = await IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
            return result.Results.Count > 0;
        }

        public async Task<bool> DeleteDataAsync(IEnumerable<string> ids, CancellationToken token)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));
            var batch = IndexBatch.Delete(ids.Select(s => new T
            {
                Id = s
            }));
            var result = await IndexClient.Documents.IndexAsync(batch, cancellationToken: token); 
            return result.Results.Count > 0;
        }

        public async Task<bool> UpdateDataAsync(IEnumerable<T> items, IEnumerable<string> ids, CancellationToken token)
        {
            if (items == null && ids == null) throw new ArgumentNullException();
            if (ids == null)
            {
                return await UpdateDataAsync(items, token);
            }
            if (items == null)
            {
                return await DeleteDataAsync(ids, token);
            }

            var actions = items.Select(IndexAction.MergeOrUpload).ToList();
            var actionDelete = ids.Select(s => IndexAction.Delete(new T
            {
                Id = s
            }));
            actions.AddRange(actionDelete);
            if (actions.Count <= 0) return false;
            var batch = IndexBatch.New(actions);
            var result = await IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
            return result.Results.Count > 0;
        }

        public virtual Task CreateOrUpdateAsync(CancellationToken token)
        {
            var index = GetIndexStructure(IndexClient.IndexName);
            return _client.Indexes.CreateOrUpdateAsync(index, cancellationToken: token);
        }

        protected abstract Index GetIndexStructure(string indexName);


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client?.Dispose();
            }
        }

        ~SearchServiceWrite()
        {
            // Finalizer calls Dispose(false)  
            Dispose(false);
        }
    }
}
