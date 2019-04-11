using NHibernate.Transform;
using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using Cloudents.Core.Extension;

namespace Cloudents.Query.Stuff
{
    public class CustomDeepTransformer<T> : IResultTransformer

    {



        public object TransformTuple(object[] tuple, string[] aliases)
        {
            // Dictionary<string, object> dic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            //for (var i = 0; i < aliases.Length; i++)
            //{
            //    if (aliases[i] != null && tuple[i] != null)
            //    {
            //        dic.Add(aliases[i], tuple[i]);
            //    }
            //}


            var result = Activator.CreateInstance(typeof(T));

            for (int i = 0; i < aliases.Length; i++)
            {
                SetProperty(aliases[i], tuple[i], result);
            }

            return result;
            //var x = new T { CultureInfo = new CultureInfo(dic["CultureInfo"].ToString()) };


            //foreach (var propertyInfo in typeof(TU).GetProperties())
            //{
            //    SetValues(dic, propertyInfo, x.CultureInfo);
            //}
            //return x;
        }

        private void SetProperty(string alias, object value, object result)
        {
            if (alias == null)
            {
                // Grouping properties in criteria are selected without alias, just ignore them.
                return;
            }

            if (value == null)
            {
                return;
            }
            var propertyInfo = result.GetType()
                .GetProperty(alias, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);

            if (propertyInfo == null)
            {
                return;
            }

            var type = propertyInfo.GetRealType(); 
            if (type.IsEnum && HandleEnum(propertyInfo, value, result))
            {

                return;

            }

            if (type == typeof(CultureInfo))
            {
                propertyInfo.SetValue(result, new CultureInfo(value.ToString()));
                return;
            }
           
            //if (value is Guid g && propertyInfo.PropertyType == typeof(string))
            //{
            //    propertyInfo.SetValue(x, g.ToString());
            //    return;

            
            //}
            //var nullableType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            //if (nullableType != null)
            //{
            //    if (nullableType.IsEnum && HandleEnum(propertyInfo, nullableType, x, value))
            //    {
            //        return;
            //    }
            //    var z = Convert.ChangeType(value, nullableType);
            //    propertyInfo.SetValue(x, z);
            //    return;
            //}
            var y = Convert.ChangeType(value, type);
            propertyInfo.SetValue(result, y);

        }

        //private static void SetValues(IReadOnlyDictionary<string, object> dic, PropertyInfo propertyInfo, object x)
        //{
        //    if (!dic.TryGetValue(propertyInfo.Name, out var value)) return;
        //    if (value == null) return;

        //    if (propertyInfo.PropertyType.IsEnum)
        //    {
        //        if (HandleEnum(propertyInfo, propertyInfo.PropertyType, x, value)) return;
        //    }
        //    if (propertyInfo.Name.Equals("CultureInfo", StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        propertyInfo.SetValue(x, new CultureInfo(value.ToString()));
        //        return;
        //    }
        //    if (value is Guid g && propertyInfo.PropertyType == typeof(string))
        //    {
        //        propertyInfo.SetValue(x, g.ToString());
        //        return;

        //    }
        //    var nullableType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
        //    if (nullableType != null)
        //    {
        //        if (nullableType.IsEnum && HandleEnum(propertyInfo, nullableType, x, value))
        //        {
        //            return;
        //        }
        //        var z = Convert.ChangeType(value, nullableType);
        //        propertyInfo.SetValue(x, z);
        //        return;
        //    }
        //    var y = Convert.ChangeType(value, propertyInfo.PropertyType);
        //    propertyInfo.SetValue(x, y);
        //}

        private static bool HandleEnum(PropertyInfo propertyInfo, object dbEnumValue, object result)
        {
            var enumType = propertyInfo.GetRealType();
            if (dbEnumValue is string str)
            {
                var e = Enum.Parse(enumType, str, true);
                propertyInfo.SetValue(result, e);
                return true;
            }

            if (dbEnumValue is int i)
            {
                var t = Enum.ToObject(enumType, i);
                propertyInfo.SetValue(result, t);
                return true;
            }

            return false;
        }
        public IList TransformList(IList collection)
        {
            return collection;
        }

    }
}