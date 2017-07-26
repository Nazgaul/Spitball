using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Zbang.Zbox.Infrastructure.Search
{
    public abstract class SearchServiceWrite<T> :  IStartable, ISearchServiceWrite<T> where T : class, ISearchObject, new()
    {

        private readonly ISearchConnection m_Connection;
        protected readonly ISearchIndexClient m_IndexClient;
        private readonly string m_IndexName;

        protected SearchServiceWrite(ISearchConnection connection, string indexName)
        {
            m_Connection = connection;
            if (m_Connection.IsDevelop)
            {

                m_IndexName = indexName + "-dev";
            }
            else
            {
                m_IndexName = indexName;
            }
            m_IndexClient = connection.SearchClient.Indexes.GetClient(m_IndexName);
        }

        public Task UpdateDataAsync(IEnumerable<T> items, CancellationToken token)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            var batch = IndexBatch.MergeOrUpload(items);
            return m_IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
        }

        public Task DeleteDataAsync(IEnumerable<string> items, CancellationToken token)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            var batch = IndexBatch.Delete(items.Select(s => new T
            {
                Id = s
            }));
            return m_IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
        }

        public async Task UpdateDataAsync(IEnumerable<T> items, IEnumerable<string> itemsToDelete, CancellationToken token)
        {
            if (items == null && itemsToDelete == null) throw new ArgumentNullException();
            if (itemsToDelete == null)
            {
                await UpdateDataAsync(items, token).ConfigureAwait(false);
                return;
            }
            if (items == null)
            {
                await DeleteDataAsync(itemsToDelete, token).ConfigureAwait(false);
                return;
            }

            var actions = items.Select(IndexAction.MergeOrUpload).ToList();
            var actionDelete = itemsToDelete.Select(s => IndexAction.Delete(new T
            {
                Id = s
            }));
            actions.AddRange(actionDelete);
            var batch = IndexBatch.New(actions);
            await m_IndexClient.Documents.IndexAsync(batch, cancellationToken: token).ConfigureAwait(false);

        }



        public abstract Index GetIndexStructure(string indexName);
        public void Start()
        {
            m_Connection.SearchClient.Indexes.CreateOrUpdate(GetIndexStructure(m_IndexName));
        }
    }
}
