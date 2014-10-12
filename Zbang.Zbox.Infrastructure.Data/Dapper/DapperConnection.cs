using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Data.Dapper
{
    public static class DapperConnection
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static async Task<IDbConnection> OpenConnectionAsync(string connectionStringName = "Zbox")
        {
            var connection = new SqlConnection(ConfigFetcher.Fetch(connectionStringName));
            await connection.OpenAsync();
            return connection;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static IDbConnection OpenConnection(string connectionStringName = "Zbox")
        {
            var connection = new SqlConnection(ConfigFetcher.Fetch(connectionStringName));
            connection.Open();
            return connection;

        }


        public static async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
        {
            using (var con = await OpenConnectionAsync())
            {

                var retVal = await con.QueryAsync<T>(sql, param);
                con.Close();
                return retVal;

            }
        }


    }
}
