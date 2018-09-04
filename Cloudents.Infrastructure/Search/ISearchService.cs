using System.Collections.Concurrent;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Search;

namespace Cloudents.Infrastructure.Search
{
    public interface ISearchService
    {
        ISearchIndexClient GetClient(string indexName);
    }

    //Taken from https://github.com/auth0/blog/blob/master/_posts/2017-01-05-azure-search-with-aspnetcore.markdown
    public class SearchService : ISearchService
    {
        private readonly SearchServiceClient _client;
        private readonly bool _isDevelop;

        //Maintaining a Dictionary of Index Clients is better performant
        private ConcurrentDictionary<string, ISearchIndexClient> indexClients;

        public SearchService(IConfigurationKeys configuration)
        {
            var key = configuration.Search;
            _isDevelop = key.IsDevelop;
            _client = new SearchServiceClient(key.Name, new SearchCredentials(key.Key));
            indexClients = new ConcurrentDictionary<string, ISearchIndexClient>();
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
                //indexName += "-dev";
            }
            return indexClients.GetOrAdd(indexName, _client.Indexes.GetClient(indexName));
        }

        public SearchServiceClient Client => _client;
    }
}