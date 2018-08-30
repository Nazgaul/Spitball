using System;
using System.Linq.Expressions;
using System.Reflection;
using Cloudents.Core.Extension;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;

namespace Cloudents.Infrastructure.Write
{
    public class FluentSearchFieldBuilder<T>
    {
        private readonly Field _field = new Field();

        public static FluentSearchFieldBuilder<T> Make => new FluentSearchFieldBuilder<T>();

        //public Field Build()
        //{
        //    return _field;
        //}


        public FluentSearchFieldBuilder<T> Map(Expression<Func<T, object>> memberExpression, DataType type)
        {
            string name = memberExpression.GetName();
            _field.Name = name;
            _field.Type = type;
            return this;

            //var z = memberExpression;
            //  return new Field();
        }


        public FluentSearchFieldBuilder<T> Map(Expression<Func<T, object>> memberExpression)
        {
            var expression = (MemberExpression)memberExpression.Body;
            string name = memberExpression.GetName();
            var propertyInfo = (PropertyInfo)expression.Member;
            _field.Name = name;
            _field.Type = GetDataType(propertyInfo.PropertyType, name);
            return this;
        }

        public FluentSearchFieldBuilder<T> IsSearchable()
        {
            _field.IsSearchable = true;
            return this;
        }
        public FluentSearchFieldBuilder<T> IsSortable()
        {
            _field.IsSortable = true;
            return this;
        }

        public FluentSearchFieldBuilder<T> IsFacetable()
        {
            _field.IsFacetable = true;
            return this;
        }

        public FluentSearchFieldBuilder<T> IsFilterable()
        {
            _field.IsFilterable = true;
            return this;
        }

        public FluentSearchFieldBuilder<T> IsKey()
        {
            _field.IsKey = true;
            return this;
        }

        public FluentSearchFieldBuilder<T> WithSearchAnalyzer(AnalyzerName analyzer)
        {
            _field.SearchAnalyzer = analyzer;
            return this;
        }

        public FluentSearchFieldBuilder<T> WithIndexAnalyzer(AnalyzerName analyzer)
        {
            _field.IndexAnalyzer = analyzer;
            return this;
        }

        public static implicit operator Field(FluentSearchFieldBuilder<T> tb)
        {
            return tb._field;
        }

        private static DataType GetDataType(Type propertyType, string propertyName)
        {
            if (propertyType == typeof(string))
            {
                return DataType.String;
            }
            if (propertyType == typeof(int))
            {
                return DataType.Int32;
            }
            if (propertyType == typeof(long))
            {
                return DataType.Int64;
            }
            if (propertyType == typeof(double))
            {
                return DataType.Double;
            }
            if (propertyType == typeof(bool))
            {
                return DataType.Boolean;
            }
            if (propertyType == typeof(DateTimeOffset) || propertyType == typeof(DateTime))
            {
                return DataType.DateTimeOffset;
            }
            if (propertyType == typeof(GeographyPoint))
            {
                return DataType.GeographyPoint;
            }
            throw new ArgumentException(string.Format("Property {0} has unsupported type {1}", propertyName, propertyType), "propertyType");
        }


    }
}