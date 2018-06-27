using System.Collections.Generic;

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
}
