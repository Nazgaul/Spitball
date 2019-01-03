using Microsoft.Azure.Search;
using System;
using System.Collections.Concurrent;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Search
{
    public sealed class SearchService : ISearchService, IDisposable
    {
        private readonly bool _isDevelop;

        //Maintaining a Dictionary of Index Clients is better performant
        private readonly ConcurrentDictionary<string, ISearchIndexClient> _indexClients;

        public SearchService(IConfigurationKeys configuration)
        {
            var key = configuration.Search;
            _isDevelop = key.IsDevelop;
            Client = new SearchServiceClient(key.Name, new SearchCredentials(key.Key));
            _indexClients = new ConcurrentDictionary<string, ISearchIndexClient>();
        }

        /// <summary>
        /// Obtains a new IndexClient and avoids Socket Exhaustion by reusing previous clients.
        /// </summary>
        /// <param name="indexName"></param>
        /// <returns></returns>
        [Obsolete("Use GetClient")]
        public ISearchIndexClient GetOldClient(string indexName)
        {
            if (_isDevelop)
            {
                //indexName += "-dev";
            }
            return _indexClients.GetOrAdd(indexName, Client.Indexes.GetClient(indexName));
        }

       

        internal SearchServiceClient Client { get; }

        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}