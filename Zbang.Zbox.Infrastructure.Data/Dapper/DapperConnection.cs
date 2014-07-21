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
    }
    //public class DapperParameters
    //{
    //    public DapperParameters(string sql, dynamic parameters)
    //    {
    //        Sql = sql;
    //        Parameters = parameters;
    //    }
    //    public string Sql { get; private set; }
    //    public dynamic Parameters { get; private set; }

    //    public DapperReturnValue<T> RetVal<T>()
    //    {

    //    }

    //}
    //public class DapperReturnValue<T>
    //{
    //    public IEnumerable<T> Values { get; set; }
    //}

    //public class DapperWrapper
    //{
    //    private List<DapperParameters> queries = new List<DapperParameters>();


    //    public async Task Excecute()
    //    {
    //        if (queries.Count == 0)
    //        {
    //            throw new NotImplementedException();
    //        }
    //        using (var conn = await DapperConnection.OpenConnection())
    //        {
    //            if (queries.Count == 1)
    //            {
    //                var query = queries[0];
    //                query.RetVal
    //                //return conn.QueryAsync((queries[0])
    //            }
    //        }
    //    }
    //    //get return something
    //    //excecute




    //}

    //public class DapperFuture<T>
    //{

    //}

}
