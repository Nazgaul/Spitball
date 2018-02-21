using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.Azure;

namespace Cloudents.Infrastructure.Write
{
    public abstract class SearchServiceWrite<T> : IDisposable, ISearchServiceWrite<T> where T : class, ISearchObject, new()
    {
        protected readonly SearchServiceClient Client;
        protected readonly ISearchIndexClient IndexClient;
        private readonly string _indexName;

        protected SearchServiceWrite(SearchServiceClient client, string indexName)
        {
            Client = client;
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
            return Client.Indexes.CreateOrUpdateAsync(GetIndexStructure(_indexName), cancellationToken: token);
        }

        //public Task UpdateDataAsync(IEnumerable<T> items, IEnumerable<string> ids, CancellationToken token)
        //{
        //    if (items == null && ids == null) throw new ArgumentNullException();
        //    if (ids == null)
        //    {
        //        return UpdateDataAsync(items, token);
        //    }
        //    if (items == null)
        //    {
        //        return DeleteDataAsync(ids, token);
        //    }

        //    var actions = items.Select(IndexAction.MergeOrUpload).ToList();
        //    var actionDelete = ids.Select(s => IndexAction.Delete(new T
        //    {
        //        Id = s
        //    }));
        //    actions.AddRange(actionDelete);
        //    if (actions.Count > 0)
        //    {
        //        var batch = IndexBatch.New(actions);
        //        return IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
        //    }
        //    return Task.CompletedTask;
        //}

        protected abstract Index GetIndexStructure(string indexName);
//        public virtual void Start()
//        {
//            try
//            {
//                Client.Indexes.Create(GetIndexStructure(_indexName));
//            }
//#pragma warning disable CC0004 // Catch block cannot be empty
//            catch (CloudException)
//            {
//                //Finish for now
//            }
//#pragma warning restore CC0004 // Catch block cannot be empty
//        }

        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}
