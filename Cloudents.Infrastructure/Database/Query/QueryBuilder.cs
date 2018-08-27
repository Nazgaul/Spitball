using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Cloudents.Core.Extension;
using NHibernate;
using NHibernate.Persister.Entity;

namespace Cloudents.Infrastructure.Database.Query
{
    public class QueryBuilder
    {
        private readonly ISessionFactory _sessionFactoryImplementor;

        public QueryBuilder(UnitOfWorkFactorySpitball unitOfWorkFactory)
        {
            _sessionFactoryImplementor = unitOfWorkFactory.GetFactory();
        }
        private readonly ConcurrentDictionary<Type, string> _tableDictionary = new ConcurrentDictionary<Type, string>();
        private readonly ConcurrentDictionary<KeyValuePair<Type, string>, string> _propertyDictionary = new ConcurrentDictionary<KeyValuePair<Type, string>, string>();
       

        public string BuildTable<T>()
        {

            return _tableDictionary.GetOrAdd(typeof(T), (f) =>
            {
                var t = _sessionFactoryImplementor.GetClassMetadata(f) as AbstractEntityPersister;
                return t.TableName;
            });
            
        }
        public string BuildProperty<T>(Expression<Func<T, object>> expression)
        {
            
            var memberName = expression.GetName();
           
            var keyValue = new KeyValuePair<Type, string>(typeof(T), memberName);

            return _propertyDictionary.GetOrAdd(keyValue, f =>
            {
                var t = _sessionFactoryImplementor.GetClassMetadata(f.Key) as AbstractEntityPersister;
                return t.GetPropertyColumnNames(f.Value).First();
            });
        }
    }
}