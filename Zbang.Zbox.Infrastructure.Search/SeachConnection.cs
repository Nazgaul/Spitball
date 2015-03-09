using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.ServiceRuntime;
using RedDog.Search;
using RedDog.Search.Http;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class SeachConnection : ISearchConnection, IDisposable
    {
        // static holder for instance, need to use lambda to construct since constructor private
        // ReSharper disable once InconsistentNaming
        //private static readonly Lazy<SeachConnection> _instance
        //    = new Lazy<SeachConnection>(() => new SeachConnection());


        private readonly ApiConnection m_Connection;
        private IndexQueryClient m_ReadClient;
        private IndexManagementClient m_IndexClient;

        public bool IsDevelop { get; private set; }
        // private to prevent direct instantiation.


        public SeachConnection(string serviceName, string serviceKey)
        {
            TraceLog.WriteInfo("on ctor of search connection");
            m_Connection = ApiConnection.Create(serviceName, serviceKey);
            IsDevelop = IsDevelopEnvironment();
        }

        private bool IsDevelopEnvironment()
        {
            //return false;
            try
            {
                if (!RoleEnvironment.IsAvailable)
                {
                    return true;
                }
                if (RoleEnvironment.IsEmulated)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(ex);
            }

            return false;
        }

        public SeachConnection(string serviceName, string serviceKey, bool isDevelop)
        {
            TraceLog.WriteInfo("on ctor of search connection");
            m_Connection = ApiConnection.Create(serviceName, serviceKey);
            IsDevelop = isDevelop;
        }

        public IndexQueryClient IndexQuery
        {
            get
            {
                if (m_ReadClient == null)
                {
                    TraceLog.WriteInfo("creating index query");
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
        //public static SeachConnection Instance
        //{
        //    get
        //    {
        //        return _instance.Value;
        //    }
        //}

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
