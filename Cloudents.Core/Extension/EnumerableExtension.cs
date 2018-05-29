using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Core.Extension
{
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


        //public static async Task<IEnumerable<T1>> SelectManyAsync<T, T1>(this IEnumerable<T> enumeration, Func<T, Task<IEnumerable<T1>>> func)
        //{
        //    return (await Task.WhenAll(enumeration.Select(func)).ConfigureAwait(false)).SelectMany(s => s);
        //}

        //https://stackoverflow.com/questions/13731796/create-batches-in-linq
        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(
            this IEnumerable<TSource> source, int size)
        {
            TSource[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new TSource[size];

                bucket[count++] = item;
                if (count != size)
                    continue;

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
                yield return bucket.Take(count);
        }
    }
}