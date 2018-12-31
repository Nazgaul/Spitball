﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Data
{
    public class DapperRepository //: IDapperRepository
    {
        private readonly string _connectionString;

        public DapperRepository(IConfigurationKeys  provider)
        {
            _connectionString = provider.Db.Db;
        }
       

        public async Task<T> WithConnectionAsync<T>(Func<IDbConnection, Task<T>> getData, CancellationToken token)
        {
            if (getData == null) throw new ArgumentNullException(nameof(getData));
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(token).ConfigureAwait(false); // Asynchronously open a connection to the database
                          // Asynchronously execute getData, which has been passed in as a Func<IDBConnection, Task<T>>
                    return await getData(connection).ConfigureAwait(false);
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

        public IDbConnection OpenConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public T WithConnection<T>(Func<IDbConnection, T> getData)
        {
            if (getData == null) throw new ArgumentNullException(nameof(getData));
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    return getData(connection);
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