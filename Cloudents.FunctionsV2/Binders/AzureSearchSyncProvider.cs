using Microsoft.Azure.Search;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using System.Collections.Concurrent;

namespace Cloudents.FunctionsV2.Binders
{
    public class AzureSearchSyncProvider : IExtensionConfigProvider
    {

        private static SearchServiceClient _client;

        private static readonly ConcurrentDictionary<string, ISearchIndexClient> IndexClients =
            new ConcurrentDictionary<string, ISearchIndexClient>();

        public void Initialize(ExtensionConfigContext context)
        {
            context
                .AddBindingRule<AzureSearchSyncAttribute>()
                .BindToCollector(CreateCollector);
        }


        private static IAsyncCollector<AzureSearchSyncOutput> CreateCollector(AzureSearchSyncAttribute attribute)
        {
            if (_client == null)
            {
                var key = attribute.Key;
                var name = attribute.Name;
                _client = new SearchServiceClient(name, new SearchCredentials(key));
            }

            var indexName = attribute.IndexName;
            bool.TryParse(attribute.IsDevelop, out var isDevelop);
            if (attribute.IsDevelopIndex)
            {
                isDevelop = true;
            }

            if (isDevelop)
            {
                indexName += "-dev";
            }

            var indexClient = IndexClients.GetOrAdd(indexName, n => _client.Indexes.GetClient(n));
            return new AzureSearchSyncAsyncCollector(indexClient);
        }
    }
}