﻿using NHibernate.Driver;
using StackExchange.Profiling.NHibernate.Drivers;

namespace Zbang.Zbox.Infrastructure.Data.Driver
{
    public class ZboxDriver : MiniProfilerSql2008ClientDriver  //Sql2008ClientDriver
    {
        //public override IDbCommand CreateCommand()
        //{
        //    return base.CreateCommand();
        //    //System.Data.SqlClient.SqlCommand c = new System.Data.SqlClient.SqlCommand();
          
            
        //    //return new ProfiledDbCommand(
        //    //    base.CreateCommand() as DbCommand,
        //    //    null,
        //    //    MiniProfiler.Current);
        //}
        
        //public override IDbConnection CreateConnection()
        //{
        //    return base.CreateConnection();
        //    //return new ProfiledDbConnection(base.CreateConnection() as DbConnection, MiniProfiler.Current);
        //}
    }
}
