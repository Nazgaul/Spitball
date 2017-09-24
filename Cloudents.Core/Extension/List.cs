using System;
using System.Collections.Generic;

namespace Cloudents.Core.Extension
{
    public static class ListExtension
    {
        public static void AddNotNull(this List<string> list, string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                list.Add(val);
            }
        }

        public static void AddNotNull(this List<string> list, string val, Func<string, string> predicate)
        {
            if (!string.IsNullOrEmpty(val))
            {
                list.Add(predicate(val));
            }
        }
    }
}
