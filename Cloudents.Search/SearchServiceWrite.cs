using Cloudents.Core.Interfaces;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Index = Microsoft.Azure.Search.Models.Index;
using ISearchObject = Cloudents.Search.Interfaces.ISearchObject;

namespace Cloudents.Search
{
    public abstract class SearchServiceWrite<T> /*:  ISearchServiceWrite<T>*/ where T : class, ISearchObject, new()
    {
        private readonly SearchServiceClient _client;
        protected readonly ISearchIndexClient IndexClient;

        private readonly ILogger _logger;


        protected SearchServiceWrite(SearchService client, ISearchIndexClient indexClient, ILogger logger)
        {
            _client = client.Client;
            IndexClient = indexClient;
            _logger = logger;
        }


        public async Task<bool> UpdateDataAsync(IEnumerable<T> items, CancellationToken token)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            var batch = IndexBatch.MergeOrUpload(items);
            var result = await IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
            foreach (var errorResult in result.Results.Where(w => !w.Succeeded))
            {
                _logger.Error($"Failed to process id {errorResult.Key} error {errorResult.ErrorMessage} on index {IndexClient.IndexName} ");
            }
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
            foreach (var errorResult in result.Results.Where(w => !w.Succeeded))
            {
                _logger.Error($"Failed to process id {errorResult.Key} error {errorResult.ErrorMessage} on index {IndexClient.IndexName} ");
            }
            return result.Results.Count > 0;
        }

        public async Task<bool> UpdateDataAsync(IEnumerable<T> items, IEnumerable<string> ids, CancellationToken token)
        {
            _logger.Info("processing");
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
            foreach (var errorResult in result.Results.Where(w => !w.Succeeded))
            {
                _logger.Error($"Failed to process id {errorResult.Key} error {errorResult.ErrorMessage} on index {IndexClient.IndexName} ");
            }

            return result.Results.Count > 0;
        }


        public async Task DeleteOldDataAsync(string filterName, DateTime timeToDelete, CancellationToken token)
        {
            const int top = 1000;
            var parameters = new SearchParameters
            {
                Filter = $"{filterName} lt {timeToDelete.ToUniversalTime():yyyy-MM-dd'T'hh:mm:ss'Z'}",
                Select = new[] { nameof(ISearchObject.Id) },
                Top = top
            };
            IList<SearchResult<T>> result;
            do
            {
                var searchRetVal = await IndexClient.Documents.SearchAsync<T>("*", parameters, cancellationToken: token);
                result = searchRetVal.Results;

                await DeleteDataAsync(result.Select(s => s.Document.Id), token);

            } while (result.Count == top);
        }

        public virtual async Task CreateOrUpdateAsync(CancellationToken token)
        {
            //var t = await _client.Indexes.GetAsync(IndexClient.IndexName, cancellationToken: token);
            //if (t == null)
            //{
            try
            {
                var index = GetIndexStructure(IndexClient.IndexName);
                await _client.Indexes.CreateOrUpdateAsync(index, cancellationToken: token);
            }
            catch (Microsoft.Rest.Azure.CloudException)
            {

            }

            //}
        }

        protected abstract Index GetIndexStructure(string indexName);





    }
}
