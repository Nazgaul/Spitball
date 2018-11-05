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


    public class FluentQueryBuilder
    {
        private readonly ISessionFactory _sessionFactoryImplementor;

        private readonly StringBuilder _fromStringBuilder = new StringBuilder();
        private string _orderBy;
        private string _paging;
        private readonly string _paramPrefix;


        private readonly List<string> _selectList = new List<string>();
        private readonly List<string> _whereList = new List<string>();
        private readonly List<char> _abcSeq = Enumerable.Range(97, 26).Select(s => (char)s).ToList();
        private readonly Dictionary<Type, string> _aliasTypes = new Dictionary<Type, string>();
        private string _column;
        private readonly Random _rand = new Random();

        public FluentQueryBuilder(UnitOfWorkFactorySpitball unitOfWorkFactory)
        {
            _sessionFactoryImplementor = unitOfWorkFactory.GetFactory();
            _paramPrefix = ":";
        }

        public FluentQueryBuilder(string sqlParam, UnitOfWorkFactorySpitball unitOfWorkFactory)
        {
            _sessionFactoryImplementor = unitOfWorkFactory.GetFactory();
            _paramPrefix = sqlParam;
        }


        public delegate FluentQueryBuilder Factory(string sqlParam);




        #region Table



        public FluentQueryBuilder InitTable<T>()
        {
            if (_fromStringBuilder.Length > 0)
            {
                throw new ArgumentException();
            }

            var tableName = Table<T>();

            _fromStringBuilder.Append($"From {tableName}");
            _fromStringBuilder.Append($" {GetAlias<T>()} ");
            return this;
        }





        public FluentQueryBuilder CustomTable(string table)
        {
            _fromStringBuilder.Append($" {table} ");
            return this;
        }

        public FluentQueryBuilder Join<TExists, TNew>(Expression<Func<TExists, object>> expressionExists,
            Expression<Func<TNew, object>> expressionNew)
        {
            var tableAlias = GetAlias<TNew>();

            var sql =
                $"join {Table<TNew>()} {tableAlias} On {ColumnAlias(expressionNew)} = {ColumnAlias(expressionExists)}";
            _fromStringBuilder.AppendLine(sql);
            return this;
            //join { _user.TableAlias}
            //on { _user.Column(x => x.Id)}={ _question.Column(x => x.User)}
        }

        public FluentQueryBuilder LeftJoin<TExists, TNew>(Expression<Func<TExists, object>> expressionExists,
            Expression<Func<TNew, object>> expressionNew)
        {
            var tableAlias = GetAlias<TNew>();

            var sql =
                $"left join {Table<TNew>()} {tableAlias} On {ColumnAlias(expressionNew)} = {ColumnAlias(expressionExists)}";
            _fromStringBuilder.AppendLine(sql);
            return this;
            //join { _user.TableAlias}
            //on { _user.Column(x => x.Id)}={ _question.Column(x => x.User)}
        }

        #endregion

        private string GetAlias<T>()
        {
            if (_aliasTypes.TryGetValue(typeof(T), out var retVal))
            {
                return retVal;
            }
            var aliasSeq = _abcSeq[0].ToString();
            _abcSeq.RemoveAt(0);

            retVal = $"{aliasSeq}{_rand.Next(1000, 9999)}";
            _aliasTypes.Add(typeof(T), retVal);
            return retVal;
        }

        /// <summary>
        /// Where list - add the statement - the statement add between
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public FluentQueryBuilder Where(string where)
        {
            _whereList.Add(where);
            return this;
        }

        public FluentQueryBuilder Select<T>(Expression<Func<T, object>> expression)
        {
            var tableAlias = GetAlias<T>();
            _selectList.Add(Column(tableAlias, expression));
            return this;
        }

        public FluentQueryBuilder Select<TIn, TOut>(Expression<Func<TIn, object>> expressionIn, Expression<Func<TOut, object>> expressionOut)
        {
            var tableAlias = GetAlias<TIn>();

            var column = Column(tableAlias, expressionIn);
            var asStatement = expressionOut.GetName();
            _selectList.Add($"{column} as {asStatement}");
            return this;
        }

        public FluentQueryBuilder Select<T>(Expression<Func<T, object>> expression, string columnAlias)
        {
            var tableAlias = GetAlias<T>();

            _column = Column(tableAlias, expression);
            _selectList.Add($"{_column} as {columnAlias}");
            return this;
        }

        public FluentQueryBuilder Select(string sql)
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
        public string TableAlias<T>()
        {
            return GetAlias<T>();
        }

        public string Column<T>(Expression<Func<T, object>> expression)
        {
            var memberName = expression.GetName();
            var keyValue = new KeyValuePair<Type, string>(typeof(T), memberName);
            var t = _sessionFactoryImplementor.GetClassMetadata(keyValue.Key) as AbstractEntityPersister;
            Debug.Assert(t != null, nameof(t) + " != null");
            return t.GetPropertyColumnNames(keyValue.Value).First();
        }

        public string ColumnAlias<T>(Expression<Func<T, object>> expression)
        {
            var tableAlias = GetAlias<T>();
            return Column(tableAlias, expression);
        }

        public string Column<T>(string tableAlias, Expression<Func<T, object>> expression)
        {
            var memberName = expression.GetName();
            var keyValue = new KeyValuePair<Type, string>(typeof(T), memberName);
            var t = _sessionFactoryImplementor.GetClassMetadata(keyValue.Key) as AbstractEntityPersister;
            Debug.Assert(t != null, nameof(t) + " != null");
            var columnName = t.GetPropertyColumnNames(keyValue.Value).First();

            //var tableAlias = GetAlias<T>();

            return $"{tableAlias}.{columnName}";
        }

        public FluentQueryBuilder AddOrder<T>(Expression<Func<T, object>> expression)
        {
            _orderBy = $"Order by {ColumnAlias(expression)}";
            return this;
        }

        public string Param(string param)
        {
            return $"{_paramPrefix}{param}";
        }

        public FluentQueryBuilder Paging(string pageSizeParam, string pageNumberParam)
        {
            _paging = $"OFFSET {Param(pageSizeParam)} * {Param(pageNumberParam)} ROWS FETCH NEXT {Param(pageSizeParam)} ROWS ONLY";
            return this;
        }


        public static implicit operator string(FluentQueryBuilder tb)
        {
            if (tb._fromStringBuilder.Length == 0)
            {
                throw new ArgumentException();
            }

            if (!string.IsNullOrEmpty(tb._paging) && string.IsNullOrEmpty(tb._orderBy))
            {
                throw new ArgumentException();
            }
            var select = "*";
            if (tb._selectList.Count > 0)
            {
                select = string.Join(",", tb._selectList);
            }

            var where = string.Empty;
            if (tb._whereList.Count > 0)
            {
                where += " where ";
                where += string.Join(" and ", tb._whereList);
            }

            return $@"select {select} {tb._fromStringBuilder} {where} {tb._orderBy} {tb._paging}";
        }
    }
}