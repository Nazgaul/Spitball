using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Search
{
    public abstract class SearchServiceWrite<T> : IDisposable, IStartable, ISearchServiceWrite<T> where T : class, ISearchObject, new()
    {
        //SearchServiceClient
        private readonly SearchServiceClient _client;
        protected readonly ISearchIndexClient IndexClient;
        private readonly string _indexName;

        protected SearchServiceWrite(SearchServiceClient client, string indexName)
        {
            _client = client;
            IndexClient = client.Indexes.GetClient(indexName);
            _indexName = indexName; // _connection.IsDevelop ? indexName + "-dev" : indexName;
            //IndexClient = connection.SearchClient.Indexes.GetClient(_indexName);
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

        public Task UpdateDataAsync(IEnumerable<T> items, IEnumerable<string> ids, CancellationToken token)
        {
            if (items == null && ids == null) throw new ArgumentNullException();
            if (ids == null)
            {
                return UpdateDataAsync(items, token);
            }
            if (items == null)
            {
                return DeleteDataAsync(ids, token);
            }

            var actions = items.Select(IndexAction.MergeOrUpload).ToList();
            var actionDelete = ids.Select(s => IndexAction.Delete(new T
            {
                Id = s
            }));
            actions.AddRange(actionDelete);
            if (actions.Count > 0)
            {
                var batch = IndexBatch.New(actions);
                return IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
            }
            return Task.CompletedTask;
        }

        protected abstract Index GetIndexStructure(string indexName);
        public void Start()
        {
            //try
            //{
                _client.Indexes.CreateOrUpdate(GetIndexStructure(_indexName));
            //}
            //catch (Exception ex)
            //{
                //Finish for now
            //}
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
