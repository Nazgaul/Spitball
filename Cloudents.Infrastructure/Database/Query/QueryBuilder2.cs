using System;
using System.Linq;
using System.Linq.Expressions;
using Cloudents.Application.Extension;
using NHibernate;
using NHibernate.Persister.Entity;

namespace Cloudents.Infrastructure.Database.Query
{
    public class QueryBuilder2<T>
    {
        private readonly ISessionFactory _sessionFactoryImplementor;
        public QueryBuilder2(UnitOfWorkFactorySpitball unitOfWorkFactory)
        {
            _sessionFactoryImplementor = unitOfWorkFactory.GetFactory();
        }

        private string _alias;

        public string Table(string alias)
        {
            _alias = alias;
            var t = _sessionFactoryImplementor.GetClassMetadata(typeof(T)) as AbstractEntityPersister;
            return $"{t.TableName} as {alias}";
        }

        public string Column(string name)
        {
            var t = _sessionFactoryImplementor.GetClassMetadata(typeof(T)) as AbstractEntityPersister;
            var columnName =  t.GetPropertyColumnNames(name).First();
            return $"{_alias}.{columnName}";
        }

        public string Column(Expression<Func<T, object>> expression)
        {
            var memberName = expression.GetName();
            return Column(memberName);
        }
    }
}