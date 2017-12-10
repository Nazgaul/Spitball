using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Search
{
    public abstract class SearchServiceWrite<T> : IDisposable, IStartable, ISearchServiceWrite<T> where T : class, ISearchObject, new()
    {
        private readonly ISearchConnection _connection;
        protected readonly ISearchIndexClient IndexClient;
        private readonly string _indexName;
        protected readonly ILogger Logger;

        protected SearchServiceWrite(ISearchConnection connection, string indexName, ILogger logger)
        {
            _connection = connection;
            Logger = logger;
            _indexName = _connection.IsDevelop ? indexName + "-dev" : indexName;
            IndexClient = connection.SearchClient.Indexes.GetClient(_indexName);
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

        public abstract Index GetIndexStructure(string indexName);
        public void Start()
        {
            try
            {
                _connection.SearchClient.Indexes.CreateOrUpdate(GetIndexStructure(_indexName));
            }
            catch (Exception ex)
            {
                if (_connection.IsDevelop)
                {
                    Logger.Exception(ex);
                }
                else
                {
                    throw;
                }
            }
        }

        public void Dispose()
        {
            IndexClient?.Dispose();
        }
    }
}
