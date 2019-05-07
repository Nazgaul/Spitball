//using System;
//using System.Collections.Generic;
//using System.Linq.Expressions;
//using System.Reflection;
//using Cloudents.Core.Extension;
//using Microsoft.Azure.Search.Models;
//using Microsoft.Spatial;

//namespace Cloudents.Search
//{
//    public class FluentSearchIndexBuilder<T>
//    {
//        private readonly List<Field> _fields = new List<Field>();
//        private string _name;

//        public FluentSearchFieldBuilder<T> Fields()
//        {
//            var t = new FluentSearchFieldBuilder<T>();
//            _fields.Add(t);
//            return t;
//        }

//        public FluentSearchIndexBuilder<T> Field(FluentSearchFieldBuilder<T> f)
//        {
//            _fields.Add(f);
//            return this;
//        }

//        public FluentSearchIndexBuilder<T> Name(string name)
//        {
//            _name = name;
//            return this;
//        }

//        public static implicit operator Index(FluentSearchIndexBuilder<T> tb)
//        {
//            return new Index(tb._name,tb._fields);
//        }

//    }
//    public class FluentSearchFieldBuilder<T>
//    {
//        private Field _field/* = new Field()*/;

//        //public static FluentSearchFieldBuilder<T> Make => new FluentSearchFieldBuilder<T>();


//        //public FluentSearchFieldBuilder<T> Map(Expression<Func<T, object>> memberExpression, DataType type)
//        //{
//        //    var name = memberExpression.GetName();
//        //    _field = new Field {Name = name, Type = type};
//        //    return this;

//        //    //var z = memberExpression;
//        //    //  return new Field();
//        //}


//        //public FluentSearchFieldBuilder<T> Map(Expression<Func<T, object>> memberExpression)
//        //{
//        //    var memberInfo = memberExpression.GetMemberInfo();

//        //    var propertyInfo = (PropertyInfo)memberInfo;
//        //    _field = new Field {Name = memberInfo.Name, Type = GetDataType(propertyInfo.PropertyType)};
//        //    return this;
//        //}



//        public FluentSearchFieldBuilder<T> IsSearchable()
//        {
//            _field.IsSearchable = true;
//            return this;
//        }
//        public FluentSearchFieldBuilder<T> IsSortable()
//        {
//            _field.IsSortable = true;
//            return this;
//        }

//        public FluentSearchFieldBuilder<T> IsFacetable()
//        {
//            _field.IsFacetable = true;
//            return this;
//        }

//        public FluentSearchFieldBuilder<T> IsFilterable()
//        {
//            _field.IsFilterable = true;
//            return this;
//        }

//        public FluentSearchFieldBuilder<T> IsKey()
//        {
//            _field.IsKey = true;
//            return this;
//        }

//        public FluentSearchFieldBuilder<T> WithSearchAnalyzer(AnalyzerName analyzer)
//        {
//            _field.SearchAnalyzer = analyzer;
//            return this;
//        }

//        public FluentSearchFieldBuilder<T> WithIndexAnalyzer(AnalyzerName analyzer)
//        {
//            _field.IndexAnalyzer = analyzer;
//            return this;
//        }

//        public static implicit operator Field(FluentSearchFieldBuilder<T> tb)
//        {
//            return tb._field;
//        }

//        private static DataType GetDataType(Type propertyType)
//        {
//            if (propertyType == typeof(string))
//            {
//                return DataType.String;
//            }
//            if (propertyType == typeof(string[]))
//            {
//                return DataType.Collection(DataType.String);
//            }
//            if (propertyType == typeof(int))
//            {
//                return DataType.Int32;
//            }
//            if (propertyType == typeof(long))
//            {
//                return DataType.Int64;
//            }
//            if (propertyType == typeof(double))
//            {
//                return DataType.Double;
//            }
//            if (propertyType == typeof(bool))
//            {
//                return DataType.Boolean;
//            }
//            if (propertyType == typeof(DateTimeOffset) || propertyType == typeof(DateTime))
//            {
//                return DataType.DateTimeOffset;
//            }
//            if (propertyType == typeof(GeographyPoint))
//            {
//                return DataType.GeographyPoint;
//            }

//            if (propertyType.IsEnum)
//            {
//                return DataType.Int32;
//            }
//            throw new ArgumentException($"Property has unsupported type {propertyType}", "propertyType");
//        }


//    }
//}