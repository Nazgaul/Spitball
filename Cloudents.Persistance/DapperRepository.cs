using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Dapper;
using Newtonsoft.Json.Linq;

namespace Cloudents.Persistence
{
    public class DapperRepository : IDapperRepository
    {
        private readonly string _connectionString;

        public DapperRepository(IConfigurationKeys  provider)
        {
            _connectionString = provider.Db.Db;
        }

        static DapperRepository()
        {
            SqlMapper.AddTypeHandler(new DapperCultureInfoTypeHandler());
            SqlMapper.AddTypeHandler(typeof(IEnumerable<string>), new JsonObjectTypeHandler());
        }
       

       

        public IDbConnection OpenConnection()
        {
            return new SqlConnection(_connectionString);
        }


        private class DapperCultureInfoTypeHandler : SqlMapper.TypeHandler<CultureInfo>
        {
            public override void SetValue(IDbDataParameter parameter, CultureInfo value)
            {
                throw new NotImplementedException();
            }

            public override CultureInfo Parse(object value)
            {
                if (value == DBNull.Value)
                {
                    return null;
                }
                return new CultureInfo(value.ToString());
            }
        }


        public class JsonObjectTypeHandler : SqlMapper.ITypeHandler
        {
            public void SetValue(IDbDataParameter parameter, object value)
            {
                throw new NotImplementedException();
            }
            public object Parse(Type destinationType, object value)
            {
                var jObject = JArray.Parse(value.ToString());
                return jObject.Children().Select(s => (string)s.First).ToList();
                // return JsonConvert.DeserializeObject(value.ToString(), destinationType);
            }
        }


    }

    
}
