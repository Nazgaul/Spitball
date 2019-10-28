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
            //var jsonObjectTypeHandler = new JsonObjectTypeHandler();
            SqlMapper.AddTypeHandler(new DapperCultureInfoTypeHandler());
            //used in tutor search query
            SqlMapper.AddTypeHandler(typeof(IEnumerable<string>), new JsonArrayTypeHandler());
            //SqlMapper.AddTypeHandler(typeof(FeedDto), new JsonObjectTypeHandler());

           
            //SqlMapper.AddTypeHandler(typeof(QuestionFeedDto), new JsonObjectTypeHandler());
            //SqlMapper.AddTypeHandler(typeof(DocumentFeedDto), new JsonObjectTypeHandler());
            
            //SqlMapper.AddTypeHandler(typeof(FeedDto), new JsonObjectTypeHandler());
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


        private class JsonArrayTypeHandler : SqlMapper.ITypeHandler
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

        //private class JsonObjectTypeHandler : SqlMapper.ITypeHandler
        //{
        //    public void SetValue(IDbDataParameter parameter, object value)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public object Parse(Type destinationType, object value)
        //    {
        //        return JsonConvert.DeserializeObject(value.ToString(), destinationType);
        //    }
        //}


    }

    
}
