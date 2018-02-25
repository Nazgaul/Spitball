using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Write
{
    public abstract class SearchServiceWrite<T> : IDisposable, ISearchServiceWrite<T> where T : class, ISearchObject, new()
    {
        private readonly SearchServiceClient _client;
        protected readonly ISearchIndexClient IndexClient;
        private readonly string _indexName;

        protected SearchServiceWrite(SearchServiceClient client, string indexName)
        {
            _client = client;
            IndexClient = client.Indexes.GetClient(indexName);
            _indexName = indexName;
        }

        public Task UpdateDataAsync(IEnumerable<T> items, CancellationToken token)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            var docs = items as IList<T> ?? items.ToList();
            if (docs.Count == 0)
            {
                return Task.CompletedTask;
            }
            var batch = IndexBatch.MergeOrUpload(docs);
            return IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
        }
        public Task DeleteDataAsync(IEnumerable<string> ids, CancellationToken token)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));
            var docs = ids as IList<string> ?? ids.ToList();
            if (docs.Count == 0)
            {
                return Task.CompletedTask;
            }
            var batch = IndexBatch.Delete(docs.Select(s => new T
            {
                Id = s
            }));
            return IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
        }

        public virtual Task CreateOrUpdateAsync(CancellationToken token)
        {
            return _client.Indexes.CreateOrUpdateAsync(GetIndexStructure(_indexName), cancellationToken: token);
        }

        protected abstract Index GetIndexStructure(string indexName);

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
