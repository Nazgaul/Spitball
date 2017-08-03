using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.Azure;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Search
{
    public abstract class SearchServiceWrite<T> : IDisposable,  IStartable, ISearchServiceWrite<T> where T : class, ISearchObject, new()
    {

        private readonly ISearchConnection m_Connection;
        protected readonly ISearchIndexClient IndexClient;
        private readonly string m_IndexName;
        protected readonly ILogger Logger;

        protected SearchServiceWrite(ISearchConnection connection, string indexName, ILogger logger)
        {
            m_Connection = connection;
            Logger = logger;
            m_IndexName = m_Connection.IsDevelop ? indexName + "-dev" : indexName;
            IndexClient = connection.SearchClient.Indexes.GetClient(m_IndexName);
        }

        public Task UpdateDataAsync(IEnumerable<T> items, CancellationToken token)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            var docs = items as IList<T> ?? items.ToList();
            if (!docs.Any())
            {
                return Task.CompletedTask;
            }
            var batch = IndexBatch.MergeOrUpload(docs);
            return IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
        }

        public Task DeleteDataAsync(IEnumerable<string> items, CancellationToken token)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            var docs = items as IList<string> ?? items.ToList();
            if (!docs.Any())
            {
                return Task.CompletedTask;
            }
            var batch = IndexBatch.Delete(docs.Select(s => new T
            {
                Id = s
            }));
            return IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
        }

        public Task UpdateDataAsync(IEnumerable<T> items, IEnumerable<string> itemsToDelete, CancellationToken token)
        {
            if (items == null && itemsToDelete == null) throw new ArgumentNullException();
            if (itemsToDelete == null)
            {
                return UpdateDataAsync(items, token);
            }
            if (items == null)
            {
                return DeleteDataAsync(itemsToDelete, token);
            }

            var actions = items.Select(IndexAction.MergeOrUpload).ToList();
            var actionDelete = itemsToDelete.Select(s => IndexAction.Delete(new T
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

        //public async Task<T> GetByIdAsync(string id, CancellationToken token)
        //{
        //    try
        //    {
        //        var t = await IndexClient.Documents.GetAsync<T>(id, cancellationToken: token).ConfigureAwait(false);
        //        return t;
        //    }
        //    catch (CloudException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
        //    {
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Exception(ex, new Dictionary<string, string>
        //        {
        //            {"service", nameof(JobsProvider)},
        //            {"id", id}
        //        });
        //        return null;
        //    }
        //}

        public abstract Index GetIndexStructure(string indexName);
        public void Start()
        {
            m_Connection.SearchClient.Indexes.CreateOrUpdate(GetIndexStructure(m_IndexName));
        }

        public void Dispose()
        {
            IndexClient?.Dispose();
        }
    }
}
