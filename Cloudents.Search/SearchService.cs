using Microsoft.Azure.Search;
using System;
using System.Collections.Concurrent;

namespace Cloudents.Search
{
    public sealed class SearchService : ISearchService, IDisposable
    {
        private readonly bool _isDevelop;

        //Maintaining a Dictionary of Index Clients is better performant
        private readonly ConcurrentDictionary<string, ISearchIndexClient> _indexClients;

        public SearchService(string key, string name, bool isDevelop)
        {

            _isDevelop = isDevelop;
            Client = new SearchServiceClient(name, new SearchCredentials(key));
            _indexClients = new ConcurrentDictionary<string, ISearchIndexClient>();
        }



        /// <summary>
        /// Obtains a new IndexClient and avoids Socket Exhaustion by reusing previous clients.
        /// </summary>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public ISearchIndexClient GetClient(string indexName)
        {
            if (_isDevelop)
            {
                indexName += "-dev";
            }

            return _indexClients.GetOrAdd(indexName, x => Client.Indexes.GetClient(x)
            );
        }

        internal SearchServiceClient Client { get; }

        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}