using System;
using System.Collections.Generic;
using System.Text;
using RedDog.Search;
using RedDog.Search.Http;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Search
{
    class SeachConnection : IDisposable
    {
        // static holder for instance, need to use lambda to construct since constructor private
        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<SeachConnection> _instance
            = new Lazy<SeachConnection>(() => new SeachConnection());


        private readonly ApiConnection m_Connection;
        private IndexQueryClient m_ReadClient;
        private IndexManagementClient m_IndexClient;

        // private to prevent direct instantiation.
        private SeachConnection()
        {
            m_Connection = ApiConnection.Create(
               ConfigFetcher.Fetch("AzureSeachServiceName"),
               ConfigFetcher.Fetch("AzureSearchKey")
               );
        }

        public IndexQueryClient IndexQuery
        {
            get
            {
                if (m_ReadClient == null)
                {
                    m_ReadClient = new IndexQueryClient(m_Connection);
                }
                return m_ReadClient;
            }
        }

        public IndexManagementClient IndexManagement
        {
            get
            {
                if (m_IndexClient == null)
                {
                    m_IndexClient = new IndexManagementClient(m_Connection);
                }
                return m_IndexClient;
            }
        }

        // accessor for instance
        public static SeachConnection Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }

        private void Dispose(bool p)
        {
            if (m_ReadClient != null)
            {
                m_ReadClient.Dispose(p);
            }
            if (m_IndexClient != null)
            {
                m_IndexClient.Dispose(p);
            }
            if (m_Connection != null)
            {
                m_Connection.Dispose(p);
            }
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

        internal static string LimitContentHighlight(IEnumerable<string> highLight)
        {
            var sb = new StringBuilder();
            foreach (var s in highLight)
            {
                sb.Append(s);
                if (sb.Length > DescriptionLength)
                {
                    break;
                }
                sb.Append("...");
            }
            return sb.ToString();
        }
    }
}
