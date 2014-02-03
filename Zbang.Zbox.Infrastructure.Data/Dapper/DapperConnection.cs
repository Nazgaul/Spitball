using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Infrastructure.Data.Dapper
{
    public static class DapperConnection
    {
        public static async Task<IDbConnection> OpenConnection()
        {
            SqlConnection connection = new SqlConnection(ConfigFetcher.Fetch("Zbox"));
            await connection.OpenAsync();
            return connection;
            
            
        }
    }

    public class DapperWrapper
    {
        //get return something
        //excecute

        
        

    }

    public class DapperFuture<T>
    {

    }

}
