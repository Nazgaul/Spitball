using System;
using Microsoft.Azure.Search;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class SearchConnection : ISearchConnection, IDisposable
    {
        private readonly SearchServiceClient m_SearchServiceClient;

        public bool IsDevelop { get; }


        public SearchConnection(string serviceName, string serviceKey)
        {
           m_SearchServiceClient = new SearchServiceClient(serviceName, new SearchCredentials(serviceKey));
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
            m_SearchServiceClient?.Dispose();
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


        public SearchServiceClient SearchClient => m_SearchServiceClient;
    }
}
