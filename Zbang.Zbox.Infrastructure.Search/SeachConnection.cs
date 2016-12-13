using System;
using Microsoft.Azure.Search;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class SeachConnection : ISearchConnection, IDisposable
    {
        // static holder for instance, need to use lambda to construct since constructor private
        // ReSharper disable once InconsistentNaming
        //private static readonly Lazy<SeachConnection> _instance
        //    = new Lazy<SeachConnection>(() => new SeachConnection());

       // SearchIndexClient 
       // private readonly ApiConnection m_Connection;
        private readonly SearchServiceClient m_SearchServiceClient;
       // private IndexQueryClient m_ReadClient;
       // private IndexManagementClient m_IndexClient;

        public bool IsDevelop { get; private set; }
        // private to prevent direct instantiation.


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
            //try
            //{
            //    if (!RoleEnvironment.IsAvailable)
            //    {
            //        return true;
            //    }
            //    if (RoleEnvironment.IsEmulated)
            //    {
            //        return true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    TraceLog.WriteError(ex);
            //}

            //return false;
        }

        //public SeachConnection(string serviceName, string serviceKey, bool isDevelop)
        //{
        //    m_SearchServiceClient = new SearchServiceClient(serviceName, new SearchCredentials(serviceKey));
        //    m_Connection = ApiConnection.Create(serviceName, serviceKey);
        //    IsDevelop = isDevelop;
        //}

        //public IndexQueryClient IndexQuery
        //{
        //    get
        //    {
        //        if (m_ReadClient == null)
        //        {
        //            m_ReadClient = new IndexQueryClient(m_Connection);
        //        }
        //        return m_ReadClient;
        //    }
        //}

        //public IndexManagementClient IndexManagement
        //{
        //    get
        //    {
        //        if (m_IndexClient == null)
        //        {
        //            m_IndexClient = new IndexManagementClient(m_Connection);
        //        }
        //        return m_IndexClient;
        //    }
        //}

       

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
