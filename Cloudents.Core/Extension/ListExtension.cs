﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Core.Extension
{
    public static class ListExtension
    {
        public static void AddNotNull(this IList<string> list, string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                list.Add(val);
            }
        }

        public static void AddNotNull(this List<string> list, string val, Func<string, string> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (!string.IsNullOrEmpty(val))
            {
                list.Add(predicate(val));
            }
        }

        public static T RemoveAndGet<T>(this IList<T> list, int index)
        {
            lock (list)
            {
                var value = list[index];
                list.RemoveAt(index);
                return value;
            }
        }
    }

    public static class EnumerableExtension
    {
        public static List<TSource> ToListIgnoreNull<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                return new List<TSource>();
            }

            if (source is List<TSource> p)
            {
                return p;
            }
            return new List<TSource>(source);
        }
    }
}
