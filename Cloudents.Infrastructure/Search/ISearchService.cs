using Cloudents.Core.Interfaces;
using Microsoft.Azure.Search;
using System;
using System.Collections.Concurrent;

namespace Cloudents.Infrastructure.Search
{
    public interface ISearchService
    {
        ISearchIndexClient GetOldClient(string indexName);
    }

    //Taken from https://github.com/auth0/blog/blob/master/_posts/2017-01-05-azure-search-with-aspnetcore.markdown
    public class SearchService : ISearchService
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
            return _indexClients.GetOrAdd(indexName, Client.Indexes.GetClient(indexName));
        }

        public SearchServiceClient Client { get; }
    }
}