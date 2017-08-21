using System;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class SearchConnection : ISearchConnection, IDisposable
    {
        public const string TutorSynonymMap = "tutor-synonym-map";

        public bool IsDevelop { get; }

        public SearchConnection(string serviceName, string serviceKey)
        {
            SearchClient = new SearchServiceClient(serviceName, new SearchCredentials(serviceKey));
            var map = new SynonymMap
            {
                Name = TutorSynonymMap,
                Format = SynonymMapFormat.Solr,
                Synonyms = "bio => biology"
            };
            SearchClient.SynonymMaps.CreateOrUpdate(map);

            IsDevelop = !IsProductionEnvironment();
        }

        private static bool IsProductionEnvironment()
        {
            if (ConfigFetcher.IsRunningOnCloud && !ConfigFetcher.IsEmulated)
            {
                return true;
            }
            bool.TryParse(ConfigFetcher.Fetch("SearchProduction"), out bool shouldUseCacheFromConfig);
            return shouldUseCacheFromConfig;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool p)
        {
            SearchClient?.Dispose();
        }

        internal static T ConvertToType<T>(object b)
        {
            if (b == null)
            {
                return default(T);
            }

            b = Convert.ChangeType(b, typeof(T));
            return (T)b;
        }

        public const int DescriptionLength = 250;

        public SearchServiceClient SearchClient { get; }
    }
}
