﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cloudents.Core.Extension
{
    public static class ObjectExtensions
    {
        //public static T ToObject<T>(this IDictionary<string, object> source)
        //    where T : class, new()
        //{
        //    var someObject = new T();
        //    var someObjectType = someObject.GetType();

        //    foreach (var item in source)
        //    {
        //        someObjectType
        //            .GetProperty(item.Key)
        //            .SetValue(someObject, item.Value, null);
        //    }

        //    return someObject;
        //}

        public static Dictionary<string, string> AsDictionary(this object source,
            BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)?.ToString() ?? string.Empty
            );

        }

        public static bool IsBetween<T>(this T item, T start, T end)
        {
            return Comparer<T>.Default.Compare(item, start) >= 0
                   && Comparer<T>.Default.Compare(item, end) <= 0;
        }
    }

    public static class DictionaryExtensions
    {
        public static string ToContentString<T, TU>(this IDictionary<T, TU> source)
        {
            return "{" + string.Join(",", source.Select(kv => kv.Key + "=" + kv.Value)) + "}";
        }
    }
}