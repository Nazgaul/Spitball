using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cloudents.Functions
{
    public class OrderPreviewComparer : IComparer<Uri>
    {
        public int Compare(Uri s1, Uri s2)
        {
            static string GetNumberFromString(Uri x) => Regex.Replace(x?.Segments.Last() ?? string.Empty, "[^\\d]", string.Empty);

            var z = GetNumberFromString(s1);
            var z2 = GetNumberFromString(s2);

            if (int.TryParse(z, out var i1) && int.TryParse(z2, out var i2))
            {
                if (i1 > i2) return 1;
                if (i1 < i2) return -1;
                if (i1 == i2) return 0;
            }

            return 0;
        }

    }
}