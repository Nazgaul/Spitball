using System;
using Microsoft.Azure.Search;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class SeachConnection : ISearchConnection, IDisposable
    {
        private readonly SearchServiceClient m_SearchServiceClient;

        public bool IsDevelop { get; }


        public SeachConnection(string serviceName, string serviceKey)
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
            bool shouldUseCacheFromConfig = true;
            bool.TryParse(ConfigFetcher.Fetch("SearchProduction"), out shouldUseCacheFromConfig);
            return shouldUseCacheFromConfig;
           
        }
       

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }

        private void Dispose(bool p)
        {
           // m_ReadClient?.Dispose(p);
           // m_IndexClient?.Dispose(p);
           // m_Connection?.Dispose();
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

        //internal static string LimitContentHighlight(IEnumerable<string> highLight)
        //{
        //    var sb = new StringBuilder();
        //    foreach (var s in highLight)
        //    {
        //        sb.Append(s);
        //        if (sb.Length > DescriptionLength)
        //        {
        //            break;
        //        }
        //        sb.Append("...");
        //    }
        //    return sb.ToString();
        //}

        public SearchServiceClient SearchClient => m_SearchServiceClient;
    }
}
