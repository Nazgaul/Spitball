using Cloudents.Core.Extension;
using NHibernate;
using NHibernate.Persister.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Cloudents.Infrastructure.Database.Query
{
    public class QueryBuilder
    {
        private readonly ISessionFactory _sessionFactoryImplementor;

        public QueryBuilder(UnitOfWorkFactorySpitball unitOfWorkFactory)
        {
            _sessionFactoryImplementor = unitOfWorkFactory.GetFactory();
        }
        //private readonly ConcurrentDictionary<Type, string> _tableDictionary = new ConcurrentDictionary<Type, string>();
        //private readonly ConcurrentDictionary<KeyValuePair<Type, string>, string> _propertyDictionary = new ConcurrentDictionary<KeyValuePair<Type, string>, string>();

        private char _alias = 'a';
        private readonly Dictionary<Type, char> _aliasTable = new Dictionary<Type, char>();

        public string BuildTable<T>()
        {

            return $"{BuildTableWithoutAlias<T>()} As {GetAlias<T>()}";
        }

        private string BuildTableWithoutAlias<T>()
        {
            var t = _sessionFactoryImplementor.GetClassMetadata(typeof(T)) as AbstractEntityPersister;
            Debug.Assert(t != null, nameof(t) + " != null");
            return $"{t.TableName}";
        }

        private char GetAlias<T>()
        {
            if (_aliasTable.TryGetValue(typeof(T), out var p))
            {
                return p;
            }

            _aliasTable[typeof(T)] = _alias++;

            return _aliasTable[typeof(T)];
        }

        //public string BuildTable<T>()
        //{

        //    var retVal = BuildTable<T>();
        //    return $"{retVal} As {alias}";
        //    //var retVal =  _tableDictionary.GetOrAdd(typeof(T), (f) =>
        //    //{
        //    //    var t = _sessionFactoryImplementor.GetClassMetadata(f) as AbstractEntityPersister;
        //    //    Debug.Assert(t != null, nameof(t) + " != null");
        //    //    //return $"{t.TableName} As {alias}";
        //    //});
        //}

        public string BuildInitVersionTable<T>(string crossTableAlias)
        {
            var aliasTable = GetAlias<T>();
            var table = BuildTable<T>();
            var t = _sessionFactoryImplementor.GetClassMetadata(typeof(T)) as AbstractEntityPersister;
            Debug.Assert(t != null, nameof(t) + " != null");
            var primaryKey = t.IdentifierColumnNames.First();

            return $" FROM {table} CROSS APPLY CHANGETABLE (VERSION {BuildTableWithoutAlias<T>()}, ({primaryKey}), ({aliasTable}.{primaryKey})) AS {crossTableAlias} ";
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
            //return _propertyDictionary.GetOrAdd(keyValue, f =>
            //{
            var t = _sessionFactoryImplementor.GetClassMetadata(keyValue.Key) as AbstractEntityPersister;
            Debug.Assert(t != null, nameof(t) + " != null");
            return t.GetPropertyColumnNames(keyValue.Value).First();
            //});
        }

        

        public string BuildProperty<T, TU>(Expression<Func<T, object>> expression, Expression<Func<TU, object>> alias)
        {
            var b = BuildProperty(expression);
            return $"{GetAlias<T>()}.{b} as {alias.GetName()}";
        }


        public string BuildJoin<T1, T2>(Expression<Func<T1, object>> expression,
            Expression<Func<T2, object>> expression2)
        {
            return $@"
            join {BuildTable<T1>()}
            on {GetAlias<T1>()}.{BuildProperty(expression)}={GetAlias<T2>()}.{BuildProperty(expression2)}";
        }
    }


    //public class QueryBuilder2
    //{
    //    private readonly ISessionFactory _sessionFactoryImplementor;

    //    public QueryBuilder2(UnitOfWorkFactorySpitball unitOfWorkFactory)
    //    {
    //        _sessionFactoryImplementor = unitOfWorkFactory.GetFactory();
    //    }

    //    private Type _firstTable;

    //    private List<string> _selectList = new List<string>();

    //    public QueryBuilder2 AddTable<T>()
    //    {
    //        if (_firstTable != null)
    //        {
    //            throw new InvalidOperationException();
    //        }
    //        _firstTable = typeof(T);
    //        return this;
    //    }

    //    public QueryBuilder2 AddSelect<T, TU>(Expression<Func<T, object>> expression,
    //        Expression<Func<TU, object>> alias)
    //    {
    //        _selectList.Add(BuildProperty(expression, alias));
    //        return this;
    //    }

    //    //public QueryBuilder2 AddSelect<T, TU>(Expression<Func<T, object>> expression,
    //    //    Expression<Func<TU, object>> alias)
    //    //{
    //    //    _selectList.Add(BuildProperty(expression, alias));
    //    //    return this;
    //    //}

    //    private char _alias = 'a';
    //    private readonly Dictionary<Type, char> _aliasTable = new Dictionary<Type, char>();

    //    public string BuildTable(Type T)
    //    {

    //        return $"{BuildTableWithoutAlias(T)} As {GetAlias(T)}";
    //    }

        

    //    private string BuildTableWithoutAlias(Type T)
    //    {
    //        var t = _sessionFactoryImplementor.GetClassMetadata(T) as AbstractEntityPersister;
    //        Debug.Assert(t != null, nameof(t) + " != null");
    //        return $"{t.TableName}";
    //    }

    //    private char GetAlias(Type T)
    //    {
    //        if (_aliasTable.TryGetValue(T, out var p))
    //        {
    //            return p;
    //        }

    //        _aliasTable[T] = _alias++;

    //        return _aliasTable[T];
    //    }


    //    private string BuildProperty<T, TU>(Expression<Func<T, object>> expression, Expression<Func<TU, object>> alias)
    //    {
    //        var b = BuildProperty(expression);
    //        return $"{GetAlias(typeof(T))}.{b} as {alias.GetName()}";
    //    }

    //    public string BuildProperty<T>(Expression<Func<T, object>> expression)
    //    {
    //        var memberName = expression.GetName();
    //        var keyValue = new KeyValuePair<Type, string>(typeof(T), memberName);
    //        //return _propertyDictionary.GetOrAdd(keyValue, f =>
    //        //{
    //        var t = _sessionFactoryImplementor.GetClassMetadata(keyValue.Key) as AbstractEntityPersister;
    //        Debug.Assert(t != null, nameof(t) + " != null");
    //        return t.GetPropertyColumnNames(keyValue.Value).First();
    //        //});
    //    }

        


    //    public static implicit operator string(QueryBuilder2 tb)
    //    {
    //        return $@"select * from 
    //            {tb.BuildTable(tb._firstTable)}";
    //    }
    //}









    public class QueryBuilder3<T>
    {
        private readonly string _alias;
        private readonly ISessionFactory _sessionFactoryImplementor;

        public QueryBuilder3(string alias, UnitOfWorkFactorySpitball unitOfWorkFactory)
        {
            _alias = alias;
            _sessionFactoryImplementor = unitOfWorkFactory.GetFactory();
        }

        public delegate QueryBuilder3<T> Factory(string alias);
        //private readonly ConcurrentDictionary<Type, string> _tableDictionary = new ConcurrentDictionary<Type, string>();
        //private readonly ConcurrentDictionary<KeyValuePair<Type, string>, string> _propertyDictionary = new ConcurrentDictionary<KeyValuePair<Type, string>, string>();

        //private char _alias = 'a';
        //private readonly Dictionary<Type, char> _aliasTable = new Dictionary<Type, char>();

        //public string BuildTable()
        //{

        //    return $"{BuildTableWithoutAlias()} As {GetAlias()}";
        //}

        public string Table
        {
            get
            {
                var t = _sessionFactoryImplementor.GetClassMetadata(typeof(T)) as AbstractEntityPersister;
                Debug.Assert(t != null, nameof(t) + " != null");
                return $"{t.TableName}";
            }
        }

        public string TableAlias
        {
            get
            {
                var t = _sessionFactoryImplementor.GetClassMetadata(typeof(T)) as AbstractEntityPersister;
                Debug.Assert(t != null, nameof(t) + " != null");
                return $"{t.TableName} as {_alias}";
            }
        }
        public string Column(Expression<Func<T, object>> expression)
        {
            var memberName = expression.GetName();
            var keyValue = new KeyValuePair<Type, string>(typeof(T), memberName);
            var t = _sessionFactoryImplementor.GetClassMetadata(keyValue.Key) as AbstractEntityPersister;
            Debug.Assert(t != null, nameof(t) + " != null");
            var columnName =  t.GetPropertyColumnNames(keyValue.Value).First();
            return $"{TableAlias}.{columnName}";
        }



        //public string BuildProperty<TU>(Expression<Func<T, object>> expression, Expression<Func<TU, object>> alias)
        //{
        //    var b = BuildProperty(expression);
        //    return $"{GetAlias()}.{b} as {alias.GetName()}";
        //}


        //public string BuildJoin<T1, T2>(Expression<Func<T1, object>> expression,
        //    Expression<Func<T2, object>> expression2)
        //{
        //    return $@"
        //    join {BuildTable<T1>()}
        //    on {GetAlias<T1>()}.{BuildProperty(expression)}={GetAlias<T2>()}.{BuildProperty(expression2)}";
        //}
    }







    public class FluentQueryBuilder
    {
        private readonly ISessionFactory _sessionFactoryImplementor;

        private StringBuilder _fromStringBuilder = new StringBuilder();

        private List<string> _selectList = new List<string>();
        private string _column;

        public FluentQueryBuilder(UnitOfWorkFactorySpitball unitOfWorkFactory)
        {
            _sessionFactoryImplementor = unitOfWorkFactory.GetFactory();
        }

        public FluentQueryBuilder AddInitTable<T>(string alias = null)
        {
            if (_fromStringBuilder.Length > 0)
            {
                throw new ArgumentException();
            }

            var tableName = Table<T>();

            _fromStringBuilder.Append($"From {tableName}");
            if (!string.IsNullOrWhiteSpace(alias))
            {
                _fromStringBuilder.Append($" {alias} ");
            }
            return this;
        }

        public FluentQueryBuilder AddCustomTable(string table)
        {
            _fromStringBuilder.Append($" {table} ");
            return this;
        }

        public FluentQueryBuilder AddSelect<T>(string tableAlias, Expression<Func<T, object>> expression)
        {
            _selectList.Add(Column(tableAlias, expression));
            return this;
        }

        public FluentQueryBuilder AddSelect<T>(string tableAlias, Expression<Func<T, object>> expression, string columnAlias)
        {
            _column = Column(tableAlias, expression);
            _selectList.Add($"{_column} {columnAlias}");
            return this;
        }

        public FluentQueryBuilder AddSelect(string sql)
        {
            _selectList.Add(sql);
            return this;
        }

        public string Table<T>()
        {
            var t = _sessionFactoryImplementor.GetClassMetadata(typeof(T)) as AbstractEntityPersister;
            Debug.Assert(t != null, nameof(t) + " != null");

            return t.TableName;
        }

        public string Column<T> (Expression<Func<T, object>> expression)
        {
            var memberName = expression.GetName();
            var keyValue = new KeyValuePair<Type, string>(typeof(T), memberName);
            var t = _sessionFactoryImplementor.GetClassMetadata(keyValue.Key) as AbstractEntityPersister;
            Debug.Assert(t != null, nameof(t) + " != null");
            return t.GetPropertyColumnNames(keyValue.Value).First();
        }
        public string Column<T>(string alias, Expression<Func<T, object>> expression)
        {
            var memberName = expression.GetName();
            var keyValue = new KeyValuePair<Type, string>(typeof(T), memberName);
            var t = _sessionFactoryImplementor.GetClassMetadata(keyValue.Key) as AbstractEntityPersister;
            Debug.Assert(t != null, nameof(t) + " != null");
            var columnName =  t.GetPropertyColumnNames(keyValue.Value).First();
            return $"{alias}.{columnName}";
        }
        /*public string BuildJoin<T1, T2>(Expression<Func<T1, object>> expression,
            Expression<Func<T2, object>> expression2)
        {
            return $@"
            join {BuildTable<T1>()}
            on {GetAlias<T1>()}.{BuildProperty(expression)}={GetAlias<T2>()}.{BuildProperty(expression2)}";
        }*/
        //public FluentQueryBuilder AddJoin<T1, T2>(Expression<Func<T1, object>> expression,
        //    Expression<Func<T2, object>> expression2)
        //{

        //}

        public static implicit operator string(FluentQueryBuilder tb)
        {
            if (tb._fromStringBuilder.Length == 0)
            {
                throw new ArgumentException();
            }

            var select = "*";
            if (tb._selectList.Count > 0)
            {
                select = string.Join(",", tb._selectList);
            }
            return $@"select {select} {tb._fromStringBuilder}";
        }
    }
}
