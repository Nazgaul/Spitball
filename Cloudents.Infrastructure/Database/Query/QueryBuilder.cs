﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
                Debug.Assert(t != null, nameof(t) + " != null");
                return t.TableName;
            });
        }

        public string BuildTable<T>(string alias)
        {
            var retVal = BuildTable<T>();
            return $"{retVal} As {alias}";
            //var retVal =  _tableDictionary.GetOrAdd(typeof(T), (f) =>
            //{
            //    var t = _sessionFactoryImplementor.GetClassMetadata(f) as AbstractEntityPersister;
            //    Debug.Assert(t != null, nameof(t) + " != null");
            //    //return $"{t.TableName} As {alias}";
            //});
        }

        public string BuildInitVersionTable<T>(string aliasTable, string crossTableAlias)
        {
            var table = BuildTable<T>();
            var t = _sessionFactoryImplementor.GetClassMetadata(typeof(T)) as AbstractEntityPersister;
            Debug.Assert(t != null, nameof(t) + " != null");
            var primaryKey = t.IdentifierColumnNames.First();

            return $" FROM {table} AS {aliasTable}  CROSS APPLY CHANGETABLE (VERSION {table}, ({primaryKey}), ({aliasTable}.{primaryKey})) AS {crossTableAlias} ";
        }

        public string BuildDiffVersionTable<T>(string aliasTable, string crossTableAlias, long version)
        {
            var table = BuildTable<T>();
            var t = _sessionFactoryImplementor.GetClassMetadata(typeof(T)) as AbstractEntityPersister;
            Debug.Assert(t != null, nameof(t) + " != null");
            var primaryKey = t.IdentifierColumnNames.First();
            return $" FROM CHANGETABLE (CHANGES {table}, {version}) AS {crossTableAlias}  LEFT OUTER JOIN {table} AS {aliasTable} ON {aliasTable}.{primaryKey} = {crossTableAlias}.{primaryKey} ";
        }

        public string BuildProperty<T>(Expression<Func<T, object>> expression)
        {
            var memberName = expression.GetName();
            var keyValue = new KeyValuePair<Type, string>(typeof(T), memberName);
            return _propertyDictionary.GetOrAdd(keyValue, f =>
            {
                var t = _sessionFactoryImplementor.GetClassMetadata(f.Key) as AbstractEntityPersister;
                Debug.Assert(t != null, nameof(t) + " != null");
                return t.GetPropertyColumnNames(f.Value).First();
            });
        }

        public string BuildProperty<T>(Expression<Func<T, object>> expression,string alias)
        {
            var b = BuildProperty(expression);
            return $"{b} as {alias}";
        }

        public string BuildProperty<T,TU>(Expression<Func<T, object>> expression, Expression<Func<TU, object>> alias)
        {
            var b = BuildProperty(expression);
            return $"{b} as {alias.GetName()}";
        }

        public string BuildProperty<T>(string tableAlias, Expression<Func<T, object>> expression)
        {
            var b = BuildProperty(expression);
            return $"{tableAlias}.{b}";
        }

        public string BuildProperty<T>(string tableAlias, Expression<Func<T, object>> expression, string alias)
        {
            var b = BuildProperty(expression,alias);
            return $"{tableAlias}.{b} as {alias}";
        }

        public string BuildProperty<T,TU>(string tableAlias, Expression<Func<T, object>> expression, Expression<Func<TU, object>> alias)
        {
            var b = BuildProperty(expression, alias);
            return $"{tableAlias}.{b}";
        }
    }
}