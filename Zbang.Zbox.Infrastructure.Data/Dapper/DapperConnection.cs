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
        public static async Task<IDbConnection> OpenConnection()
        {
            var connection = new SqlConnection(ConfigFetcher.Fetch("Zbox"));
            await connection.OpenAsync();
            return connection;
            
            
        }
    }
    public class DapperParameters
    {
        public DapperParameters(string sql, dynamic parameters)
        {
            Sql = sql;
            Parameters = parameters;
        }
        public string Sql { get; private set; }
        public dynamic Parameters { get; private set; }
        //public DapperReturnValue MyProperty { get; private set; }

    }
    //public class DapperReturnValue<T>
    //{

    //}

    public class DapperWrapper
    {
        private List<DapperParameters> queries = new List<DapperParameters>();


        public async Task Excecute()
        {
            using (var conn = await DapperConnection.OpenConnection())
            {
                using (var grid = conn.QueryMultiple("s"))
                {
                    
                }
            }
        }
        //get return something
        //excecute

        
        

    }

    public class DapperFuture<T>
    {

    }

}
