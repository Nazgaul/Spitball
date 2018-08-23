using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Cloudents.Core.Entities.Search;
using Cloudents.Infrastructure.Search;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;

namespace Cloudents.Infrastructure.Write
{
    public class QuestionSearchWrite : SearchServiceWrite<Question>
    {
        public QuestionSearchWrite(SearchService client, string indexName) : base(client, indexName)
        {
        }

        protected override Index GetIndexStructure(string indexName)
        {
            //var z = FluentSearchField<Question>.Make;
            //var t = new FluentSearch();
            //  t.Map<Question>(x => x.Id);
            //   Microsoft.Azure.Management.Search.Fluent.
            return new Index()
            {
                Name = indexName,
                Fields = new List<Field>
                {

                   GetFieldBuilder.Map(x=>x.Id).IsKey().Build(),

                   GetFieldBuilder.Map(x=>x.UserId).Build(),
                   GetFieldBuilder.Map(x=>x.UserName).Build(),
                   GetFieldBuilder.Map(x=>x.UserImage).Build(),

                   GetFieldBuilder.Map(x=>x.AnswerCount).IsFilterable().IsFacetable()
                       .Build(),
                   GetFieldBuilder.Map(x=>x.FilesCount).Build(),


                   GetFieldBuilder.Map(x=>x.DateTime).IsSortable().Build(),
                   GetFieldBuilder.Map(x=>x.HasCorrectAnswer).IsFilterable().Build(),
                   GetFieldBuilder.Map(x=>x.Price).Build(),
                   GetFieldBuilder.Map(x=>x.Text).IsSearchable().Build(),
                   GetFieldBuilder.Map(x=>x.Color).Build(),
                   GetFieldBuilder.Map(x=>x.Subject).IsFilterable().IsFacetable().Build(),

                }
            };
        }
    }


    public class FluentSearchField<T>
    {
        private Field _field = new Field();

        public static FluentSearchField<T> Make => new FluentSearchField<T>();

        public Field Build()
        {
            return _field;
        }


        public FluentSearchField<T> Map(Expression<Func<T, object>> memberExpression, DataType type)
        {
            var expression = (MemberExpression)memberExpression.Body;
            string name = expression.Member.Name;
            _field.Name = name;
            _field.Type = type;
            return this;

            //var z = memberExpression;
            //  return new Field();
        }


        public FluentSearchField<T> Map(Expression<Func<T, object>> memberExpression)
        {
            var expression = (MemberExpression)memberExpression.Body;
            string name = expression.Member.Name;


            var propertyInfo = (PropertyInfo)expression.Member;
            _field.Name = name;
            _field.Type = GetDataType(propertyInfo.PropertyType, name);
            return this;

            //var z = memberExpression;
            //  return new Field();
        }

        public FluentSearchField<T> IsSearchable()
        {
            _field.IsSearchable = true;
            return this;
        }
        public FluentSearchField<T> IsSortable()
        {
            _field.IsSortable = true;
            return this;
        }

        public FluentSearchField<T> IsFacetable()
        {
            _field.IsFacetable = true;
            return this;
        }

        public FluentSearchField<T> IsFilterable()
        {
            _field.IsFilterable = true;
            return this;
        }

        public FluentSearchField<T> IsKey()
        {
            _field.IsKey = true;
            return this;
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