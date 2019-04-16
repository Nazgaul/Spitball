//using NHibernate.Transform;
//using System;
//using System.Collections;
//using System.Globalization;
//using System.Reflection;
//using Cloudents.Core.Extension;

//namespace Cloudents.Query.Stuff
//{
//    //public static class SbTransformers
//    //{
//    //    public static IResultTransformer AliasToBeanImprovement<T>()
//    //    {
//    //        return new CustomDeepTransformer(typeof(T));
//    //    }
//    //}

//    //public class CustomDeepTransformer : IResultTransformer
//    //{
//    //    private readonly Type _type;
//    //    public CustomDeepTransformer(Type type)
//    //    {
//    //        _type = type;
//    //    }

       
//    //    public object TransformTuple(object[] tuple, string[] aliases)
//    //    {
//    //        // Dictionary<string, object> dic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
//    //        //for (var i = 0; i < aliases.Length; i++)
//    //        //{
//    //        //    if (aliases[i] != null && tuple[i] != null)
//    //        //    {
//    //        //        dic.Add(aliases[i], tuple[i]);
//    //        //    }
//    //        //}


//    //        var result = Activator.CreateInstance(_type);

//    //        for (int i = 0; i < aliases.Length; i++)
//    //        {
//    //            SetProperty(aliases[i], tuple[i], result);
//    //        }

//    //        return result;
//    //        //var x = new T { CultureInfo = new CultureInfo(dic["CultureInfo"].ToString()) };


//    //        //foreach (var propertyInfo in typeof(TU).GetProperties())
//    //        //{
//    //        //    SetValues(dic, propertyInfo, x.CultureInfo);
//    //        //}
//    //        //return x;
//    //    }

//    //    internal static void SetProperty(string alias, object dbValue, object result)
//    //    {
//    //        if (alias == null)
//    //        {
//    //            // Grouping properties in criteria are selected without alias, just ignore them.
//    //            return;
//    //        }

//    //        if (dbValue == null)
//    //        {
//    //            return;
//    //        }
//    //        var propertyInfo = result.GetType()
//    //            .GetProperty(alias, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);

//    //        if (propertyInfo == null)
//    //        {
//    //            return;
//    //        }

//    //        var type = propertyInfo.GetRealType(); 
//    //        if (type.IsEnum && HandleEnum(propertyInfo, dbValue, result))
//    //        {

//    //            return;

//    //        }

//    //        if (type == typeof(CultureInfo))
//    //        {
//    //            propertyInfo.SetValue(result, new CultureInfo(dbValue.ToString()));
//    //            return;
//    //        }
          
//    //        var y = Convert.ChangeType(dbValue, type);
//    //        propertyInfo.SetValue(result, y);

//    //    }

//    //    //private static void SetValues(IReadOnlyDictionary<string, object> dic, PropertyInfo propertyInfo, object x)
//    //    //{
//    //    //    if (!dic.TryGetValue(propertyInfo.Name, out var value)) return;
//    //    //    if (value == null) return;

//    //    //    if (propertyInfo.PropertyType.IsEnum)
//    //    //    {
//    //    //        if (HandleEnum(propertyInfo, propertyInfo.PropertyType, x, value)) return;
//    //    //    }
//    //    //    if (propertyInfo.Name.Equals("CultureInfo", StringComparison.InvariantCultureIgnoreCase))
//    //    //    {
//    //    //        propertyInfo.SetValue(x, new CultureInfo(value.ToString()));
//    //    //        return;
//    //    //    }
//    //    //    if (value is Guid g && propertyInfo.PropertyType == typeof(string))
//    //    //    {
//    //    //        propertyInfo.SetValue(x, g.ToString());
//    //    //        return;

//    //    //    }
//    //    //    var nullableType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
//    //    //    if (nullableType != null)
//    //    //    {
//    //    //        if (nullableType.IsEnum && HandleEnum(propertyInfo, nullableType, x, value))
//    //    //        {
//    //    //            return;
//    //    //        }
//    //    //        var z = Convert.ChangeType(value, nullableType);
//    //    //        propertyInfo.SetValue(x, z);
//    //    //        return;
//    //    //    }
//    //    //    var y = Convert.ChangeType(value, propertyInfo.PropertyType);
//    //    //    propertyInfo.SetValue(x, y);
//    //    //}

        
//    //    public IList TransformList(IList collection)
//    //    {
//    //        return collection;
//    //    }

//    //}
//}