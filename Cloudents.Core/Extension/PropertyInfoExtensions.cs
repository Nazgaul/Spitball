using System;
using System.Globalization;
using System.Reflection;

namespace Cloudents.Core.Extension
{
    //    public static class EnumerableExtension
    //    {
    //        //public static List<TSource> ToListIgnoreNull<TSource>(this IEnumerable<TSource> source)
    //        //{
    //        //    if (source == null)
    //        //    {
    //        //        return new List<TSource>();
    //        //    }

    //        //    if (source is List<TSource> p)
    //        //    {
    //        //        return p;
    //        //    }
    //        //    return new List<TSource>(source);
    //        //}

    //        //https://stackoverflow.com/questions/13731796/create-batches-in-linq
    //        /*public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(
    //            this IEnumerable<TSource> source, int size)
    //        {
    //            TSource[] bucket = null;
    //            var count = 0;

    //            foreach (var item in source)
    //            {
    //                if (bucket == null)
    //                    bucket = new TSource[size];

    //                bucket[count++] = item;
    //                if (count != size)
    //                    continue;

    //                yield return bucket;

    //                bucket = null;
    //                count = 0;
    //            }

    //            if (bucket != null && count > 0)
    //                yield return bucket.Take(count);
    //        }*/
    //    }

    public static class PropertyInfoExtensions
    {
        public static Type GetRealType(this PropertyInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            var propertyType = info.PropertyType;
            return Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        }

        public static void SetValueExtension(this PropertyInfo info, object obj, object value)
        {
            var type = info.GetRealType();
            if (type.IsEnum && HandleEnum(info, obj, value))
            {

                return;

            }

            if (type == typeof(CultureInfo))
            {
                info.SetValue(obj, new CultureInfo(value.ToString()));
                return;
            }

            if (type.IsSubclassOf(typeof(Enumeration)) && value is int)
            {
                //var val = Convert.ToInt32(value);
                var methodName = nameof(Enumeration.FromValue);
                var method = typeof(Enumeration).GetMethod(methodName,
                    BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(new[] { type });
                var val = method.Invoke(null, new[] { value });
                info.SetValue(obj, val);
                return;
            }
            info.SetValue(obj, value);
        }

        private static bool HandleEnum(PropertyInfo propertyInfo, object obj, object value)
        {
            var enumType = propertyInfo.GetRealType();
            if (value is string str)
            {
                var e = System.Enum.Parse(enumType, str, true);
                propertyInfo.SetValue(obj, e);
                return true;
            }

            if (value is int i)
            {
                var t = System.Enum.ToObject(enumType, i);
                propertyInfo.SetValue(obj, t);
                return true;
            }

            return false;
        }
    }
}