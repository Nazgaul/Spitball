using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Dapper;

namespace Cloudents.Infrastructure.Data
{
    public class DapperRepository : IReadRepository
    {
        private readonly string m_ConnectionString;

        public DapperRepository(string connectionString)
        {
            m_ConnectionString = connectionString;
        }

        public async Task<T> WithConnectionAsync<T>(Func<IDbConnection, Task<T>> getData, CancellationToken token)
        {
            try
            {
                using (var connection = new SqlConnection(m_ConnectionString))
                {
                    await connection.OpenAsync(token).ConfigureAwait(false); // Asynchronously open a connection to the database
                    var data =  await getData(connection).ConfigureAwait(false); // Asynchronously execute getData, which has been passed in as a Func<IDBConnection, Task<T>>
                    return data;
                }
            }
            catch (TimeoutException ex)
            {
                ex.Data.Add("additional data", $"{GetType().FullName}.WithConnection() experienced a SQL timeout");
                throw;
            }
            catch (SqlException ex)
            {
                ex.Data.Add("additional data", $"{GetType().FullName}.WithConnection()  experienced a SQL exception (not a timeout)");
                throw;
            }
        }
    }
}
