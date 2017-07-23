using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class SearchServiceWrite<T> : ISearchServiceWrite<T> where T : ISearchObject
    {

        private readonly ISearchConnection m_Connection;
        private readonly ISearchIndexClient m_IndexClient;
        private readonly string m_IndexName;

        public SearchServiceWrite(ISearchConnection connection)
        {
            m_Connection = connection;
            if (m_Connection.IsDevelop)
            {
                
               // m_IndexName = T.IndexName + "-dev";
            }
            m_IndexClient = connection.SearchClient.Indexes.GetClient(m_IndexName);
        }

        public Task UpdateDataAsync(T items, CancellationToken token)
        {
            
            throw new NotImplementedException();
        }

        public Task DeleteDataAsync(T items, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDataAsync(T items, string itemsToDelete, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
